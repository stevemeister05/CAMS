using CAMS.Application.Common;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.Security;
using CAMS.Application.Common.Validation;
using CAMS.Application.Member;
using CAMS.Application.Registration.DTOs;
using CAMS.Application.Settings;
using CAMS.Application.User;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace CAMS.Application.Registration;

public class RegistrationService : IRegistrationService
{
	private readonly IMemberRepository _memberRepository;
	private readonly IRegistrationRequestRepository _registrationRequestRepository;
	private readonly IUserService _userService;

	private readonly IPasswordHasher _passwordHasher;
	private readonly IUnitOfWork _unitOfWork;
	private readonly RegistrationSettings _registrationSettings;

	public RegistrationService(
		IMemberRepository memberRepository,
		IRegistrationRequestRepository registrationRequestRepository,
		IUserService userService,
		IOptions<RegistrationSettings> registrationSettings,
		IPasswordHasher passwordHasher,
		IUnitOfWork unitOfWork)
	{
		_memberRepository = memberRepository;
		_registrationRequestRepository = registrationRequestRepository;
		_userService = userService;
		_registrationSettings = registrationSettings.Value;
		_passwordHasher = passwordHasher;
		_unitOfWork = unitOfWork;
	}

	public async Task<RegistrationResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var registration =
			await _registrationRequestRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (registration is null)
		{
			throw new NotFoundException(
				"Registration request not found.");
		}

		return MapToResponse(registration);
	}

	public async Task<PagedResult<RegistrationResponse>> SearchAsync(
		PagedRequest<RegistrationFilter> request,
		CancellationToken cancellationToken = default)
	{
		var result =
			await _registrationRequestRepository.SearchAsync(
				request,
				cancellationToken);

		var items = result.Items
			.Select(MapToResponse)
			.ToList();

		return new PagedResult<RegistrationResponse>(
			items,
			result.TotalCount,
			result.Page,
			result.PageSize);
	}

	public async Task<RegistrationResponse> RegisterAsync(
		RegisterRequest request,
		CancellationToken cancellationToken = default)
	{
		var mobileNumber =
			request.MobileNumber.Trim();

		Validator.ThrowIfNotValidPhoneNumber(
			mobileNumber);

		// Check if the mobile number already belongs to a member.
		var existingMember =
			await _memberRepository.GetByMobileNumberAsync(
				mobileNumber,
				cancellationToken);

		if (existingMember is not null)
		{
			throw new ConflictException(
				"A member with this mobile number already exists.");
		}

		// Check if there is already an existing registration request.
		var existingRegistration =
			await _registrationRequestRepository.GetByMobileNumberAsync(
				mobileNumber,
				cancellationToken);

		if (existingRegistration is not null)
		{
			throw new ConflictException(
				"A registration request for this mobile number already exists.");
		}

		var registrationRequest =
			new RegistrationRequest
			{
				FirstName =
					request.FirstName.Trim(),

				MiddleName =
					request.MiddleName?.Trim(),

				LastName =
					request.LastName.Trim(),

				MobileNumber =
					mobileNumber,

				PasswordHash =
					_passwordHasher.Hash(
						request.Password),

				Address =
					request.Address?.Trim(),

				BirthDate =
					request.BirthDate,

				Gender =
					request.Gender?.Trim(),

				Status =
					RegistrationStatus.Pending,

				RegisteredAt =
					DateTime.UtcNow
			};

		await _registrationRequestRepository.AddAsync(
			registrationRequest,
			cancellationToken);

		/*
		 * If administrator approval is required,
		 * leave the registration as Pending.
		 */
		if (_registrationSettings.RequireAdminApproval)
		{
			await _unitOfWork.SaveChangesAsync(
				cancellationToken);
		}
		else
		{
			/*
			 * If administrator approval is not required,
			 * immediately create the Member and ApplicationUser
			 * and mark the registration as Approved.
			 */
			await ApproveRegistrationAsync(
				registrationRequest,
				cancellationToken);
		}

		return MapToResponse(
			registrationRequest);
	}

	public async Task<RegistrationResponse> ApproveAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var registration =
			await _registrationRequestRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (registration is null)
		{
			throw new NotFoundException(
				"Registration request not found.");
		}

		if (registration.Status !=
			RegistrationStatus.Pending)
		{
			throw new ConflictException(
				"Only pending registration requests can be approved.");
		}

		await ApproveRegistrationAsync(
			registration,
			cancellationToken);

		return MapToResponse(
			registration);
	}

	public async Task RejectAsync(
		Guid id,
		RejectRegistrationRequest request,
		CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(
			request.Reason))
		{
			throw new ValidationException(
				"Rejection reason is required.");
		}

		var registration =
			await _registrationRequestRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (registration is null)
		{
			throw new NotFoundException(
				"Registration request not found.");
		}

		if (registration.Status !=
			RegistrationStatus.Pending)
		{
			throw new ConflictException(
				"Only pending registration requests can be rejected.");
		}

		registration.Status =
			RegistrationStatus.Rejected;

		registration.RejectedAt =
			DateTime.UtcNow;

		registration.RejectionReason =
			request.Reason.Trim();

		_registrationRequestRepository.Update(
			registration);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);
	}

	private async Task ApproveRegistrationAsync(
		RegistrationRequest registration,
		CancellationToken cancellationToken)
	{
		await using var transaction =
			await _unitOfWork.BeginTransactionAsync(
				cancellationToken);

		try
		{
			/*
			 * Check again during approval.
			 *
			 * This protects against another member being created
			 * after the initial registration check.
			 */
			var existingMember =
				await _memberRepository.GetByMobileNumberAsync(
					registration.MobileNumber,
					cancellationToken);

			if (existingMember is not null)
			{
				throw new ConflictException(
					"A member with this mobile number already exists.");
			}

			/*
			 * Create the Member.
			 *
			 * The Member uses Guid as its primary key, so
			 * member.Id is available immediately.
			 */
			var member =
				MapToMember(registration);

			await _memberRepository.AddAsync(
				member,
				cancellationToken);

			/*
			 * Create the ApplicationUser.
			 *
			 * The password was already hashed during registration.
			 */
			await _userService.CreateMemberUserAsync(
				member.Id,
				registration.PasswordHash,
				cancellationToken);

			/*
			 * Mark the registration as approved.
			 */
			registration.Status =
				RegistrationStatus.Approved;

			registration.ApprovedAt =
				DateTime.UtcNow;

			_registrationRequestRepository.Update(
				registration);

			/*
			 * Persist all changes together:
			 *
			 * - Member
			 * - ApplicationUser
			 * - User role
			 * - RegistrationRequest
			 */
			await _unitOfWork.SaveChangesAsync(
				cancellationToken);

			await transaction.CommitAsync(
				cancellationToken);
		}
		catch
		{
			await transaction.RollbackAsync(
				cancellationToken);

			throw;
		}
	}

	private static Domain.Entities.Member MapToMember(
		RegistrationRequest registration)
	{
		return new Domain.Entities.Member
		{
			FirstName =
				registration.FirstName,

			MiddleName =
				string.IsNullOrWhiteSpace(
					registration.MiddleName)
					? null
					: registration.MiddleName.Trim(),

			LastName =
				registration.LastName,

			MobileNumber =
				registration.MobileNumber,

			Address =
				string.IsNullOrWhiteSpace(
					registration.Address)
					? null
					: registration.Address.Trim(),

			BirthDate =
				registration.BirthDate,

			Gender =
				string.IsNullOrWhiteSpace(
					registration.Gender)
					? null
					: registration.Gender.Trim(),

			IsActive = true,

			CreatedAt =
				DateTime.UtcNow
		};
	}

	private static RegistrationResponse MapToResponse(
		RegistrationRequest registration)
	{
		return new RegistrationResponse
		{
			RegistrationId = registration.Id,
			MobileNumber = registration.MobileNumber,
			FirstName = registration.FirstName,
			MiddleName = registration.MiddleName,
			LastName = registration.LastName,
			Status = registration.Status,
			RegisteredAt = registration.RegisteredAt
		};
	}
}
