using CAMS.Application.Common;
using CAMS.Application.Common.Clocking;
using CAMS.Application.Common.Exceptions;
using CAMS.Application.Common.Pagination;
using CAMS.Application.Common.Validation;
using CAMS.Application.Member.DTOs;

namespace CAMS.Application.Member;

public sealed class MemberService : IMemberService
{
	private readonly IMemberRepository _memberRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IApplicationClock _clock;

	public MemberService(
		IMemberRepository memberRepository,
		IUnitOfWork unitOfWork,
		IApplicationClock clock)
	{
		_memberRepository = memberRepository;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<MemberResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var member = await _memberRepository.GetByIdAsync(
			id,
			cancellationToken);

		if (member is null)
		{
			throw new NotFoundException("Member not found.");
		}

		return MapToResponse(member);
	}

	public async Task<PagedResult<MemberResponse>> SearchAsync(
	PagedRequest<MemberFilter> request,
	CancellationToken cancellationToken = default)
	{
		var result = await _memberRepository.SearchAsync(
			request,
			cancellationToken);

		var items = result.Items
			.Select(MapToResponse)
			.ToList();

		return new PagedResult<MemberResponse>(
			items,
			result.TotalCount,
			result.Page,
			result.PageSize);
	}

	public async Task<MemberResponse> CreateAsync(
		CreateMemberRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateCreateRequest(request);

		var mobileNumber = request.MobileNumber.Trim();

		Validator.ThrowIfNotValidPhoneNumber(mobileNumber);

		var existingMember =
			await _memberRepository.GetByMobileNumberAsync(
				mobileNumber,
				cancellationToken);

		if (existingMember is not null)
		{
			throw new ConflictException(
				"A member with this mobile number already exists.");
		}

		var member = new Domain.Entities.Member
		{
			FirstName = request.FirstName.Trim(),

			MiddleName = string.IsNullOrWhiteSpace(request.MiddleName)
				? null
				: request.MiddleName.Trim(),

			LastName = request.LastName.Trim(),

			MobileNumber = mobileNumber,

			BirthDate = request.BirthDate,

			Gender = string.IsNullOrWhiteSpace(request.Gender)
				? null
				: request.Gender.Trim(),

			Address = string.IsNullOrWhiteSpace(request.Address)
				? null
				: request.Address.Trim(),

			IsActive = true,

			CreatedAt = DateTime.UtcNow
		};

		await _memberRepository.AddAsync(
			member,
			cancellationToken);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		return MapToResponse(member);
	}

	public async Task<MemberResponse?> UpdateAsync(
		Guid id,
		UpdateMemberRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateUpdateRequest(request);

		var member = await _memberRepository.GetByIdAsync(
			id,
			cancellationToken);

		if (member is null)
		{
			return null;
		}

		var mobileNumber = request.MobileNumber.Trim();

		var existingMember =
			await _memberRepository.GetByMobileNumberAsync(
				mobileNumber,
				cancellationToken);

		if (existingMember is not null &&
			existingMember.Id != id)
		{
			throw new ConflictException(
				"A member with this mobile number already exists.");
		}

		member.FirstName = request.FirstName.Trim();

		member.MiddleName = string.IsNullOrWhiteSpace(
			request.MiddleName)
				? null
				: request.MiddleName.Trim();

		member.LastName = request.LastName.Trim();

		member.MobileNumber = mobileNumber;

		member.BirthDate = request.BirthDate;

		member.Gender = string.IsNullOrWhiteSpace(
			request.Gender)
				? null
				: request.Gender.Trim();

		member.Address = string.IsNullOrWhiteSpace(
			request.Address)
				? null
				: request.Address.Trim();

		member.UpdatedAt = DateTime.UtcNow;

		_memberRepository.Update(member);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		return MapToResponse(member);
	}

	public async Task<bool> DeleteAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var member = await _memberRepository.GetByIdAsync(
			id,
			cancellationToken);

		if (member is null)
		{
			return false;
		}

		_memberRepository.Delete(member);

		await _unitOfWork.SaveChangesAsync(
			cancellationToken);

		return true;
	}

	public async Task<MemberResponse> ActivateAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var member =
			await _memberRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (member is null)
		{
			throw new NotFoundException(
				"Member was not found.");
		}


		if (member.IsActive)
		{
			return MapToResponse(
				member);
		}


		member.IsActive =
			true;

		member.UpdatedAt =
			_clock.UtcNow;


		await _unitOfWork.SaveChangesAsync(
			cancellationToken);


		return MapToResponse(
			member);
	}

	public async Task<MemberResponse> DeactivateAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		var member =
			await _memberRepository.GetByIdAsync(
				id,
				cancellationToken);

		if (member is null)
		{
			throw new NotFoundException(
				"Member was not found.");
		}


		if (!member.IsActive)
		{
			return MapToResponse(
				member);
		}


		member.IsActive =
			false;

		member.UpdatedAt =
			_clock.UtcNow;


		await _unitOfWork.SaveChangesAsync(
			cancellationToken);


		return MapToResponse(
			member);
	}

	private static void ValidateCreateRequest(
		CreateMemberRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.FirstName))
		{
			throw new ValidationException(
				"First name is required.");
		}

		if (string.IsNullOrWhiteSpace(request.LastName))
		{
			throw new ValidationException(
				"Last name is required.");
		}

		if (string.IsNullOrWhiteSpace(request.MobileNumber))
		{
			throw new ValidationException(
				"Mobile number is required.");
		}
	}

	private static void ValidateUpdateRequest(
		UpdateMemberRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.FirstName))
		{
			throw new ValidationException(
				"First name is required.");
		}

		if (string.IsNullOrWhiteSpace(request.LastName))
		{
			throw new ValidationException(
				"Last name is required.");
		}

		if (string.IsNullOrWhiteSpace(request.MobileNumber))
		{
			throw new ValidationException(
				"Mobile number is required.");
		}
	}

	private static MemberResponse MapToResponse(
		Domain.Entities.Member member)
	{
		return new MemberResponse
		{
			Id = member.Id,

			FirstName = member.FirstName,

			MiddleName = member.MiddleName,

			LastName = member.LastName,

			FullName = BuildFullName(member),

			MobileNumber = member.MobileNumber,

			BirthDate = member.BirthDate,

			Gender = member.Gender,

			Address = member.Address,

			IsActive = member.IsActive,

			CreatedAt = member.CreatedAt,

			UpdatedAt = member.UpdatedAt
		};
	}

	private static string BuildFullName(
		Domain.Entities.Member member)
	{
		return string.Join(
			" ",
			new[]
			{
				member.FirstName,
				member.MiddleName,
				member.LastName
			}
			.Where(x => !string.IsNullOrWhiteSpace(x)));
	}
}
