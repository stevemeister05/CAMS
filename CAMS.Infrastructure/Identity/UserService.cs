using CAMS.Application.Common.Exceptions;
using CAMS.Application.Common.Security;
using CAMS.Application.Member;
using CAMS.Application.User;
using CAMS.Application.User.DTOs;
using CAMS.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Identity;

public class UserService : IUserService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IMemberRepository _memberRepository;
	private readonly IPasswordHasher _passwordHasher;

	public UserService(
		UserManager<ApplicationUser> userManager,
		IMemberRepository memberRepository,
		IPasswordHasher passwordHasher)
	{
		_userManager = userManager;
		_memberRepository = memberRepository;
		_passwordHasher = passwordHasher;
	}

	public async Task<UserResponse> CreateMemberUserAsync(
		Guid memberId,
		string passwordHash,
		CancellationToken cancellationToken = default)
	{
		if (memberId == Guid.Empty)
		{
			throw new ValidationException(
				"Member ID is required.");
		}

		ValidatePasswordHash(passwordHash);

		var member =
			await _memberRepository.GetByIdAsync(
				memberId,
				cancellationToken);

		if (member is null)
		{
			throw new NotFoundException(
				"Member was not found.");
		}

		if (string.IsNullOrWhiteSpace(
			member.MobileNumber))
		{
			throw new ValidationException(
				"Member must have a mobile number " +
				"before a user account can be created.");
		}

		var mobileNumber =
			member.MobileNumber.Trim();

		var existingMemberUser =
			await _userManager.Users
				.FirstOrDefaultAsync(
					x => x.MemberId == memberId,
					cancellationToken);

		if (existingMemberUser is not null)
		{
			throw new ConflictException(
				"This member already has a user account.");
		}

		return await CreateUserAsync(
			userName: mobileNumber,
			passwordHash: passwordHash,
			role: ApplicationRoles.Member,
			memberId: member.Id,
			mustChangePassword: false,
			cancellationToken);
	}

	public async Task<UserResponse> CreateAdministratorAsync(
		CreateStaffUserRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateStaffRequest(request);

		var passwordHash =_passwordHasher.Hash(request.Password);

		return await CreateUserAsync(
			userName: request.UserName.Trim(),
			passwordHash: passwordHash,
			role: ApplicationRoles.Administrator,
			memberId: null,
			mustChangePassword: true,
			cancellationToken);
	}

	public async Task<UserResponse> CreateAttendanceStaffAsync(
		CreateStaffUserRequest request,
		CancellationToken cancellationToken = default)
	{
		ValidateStaffRequest(request);

		var passwordHash =_passwordHasher.Hash(request.Password);

		return await CreateUserAsync(
			userName: request.UserName.Trim(),
			passwordHash: passwordHash,
			role: ApplicationRoles.AttendanceStaff,
			memberId: null,
			mustChangePassword: true,
			cancellationToken);
	}

	public async Task ChangePasswordAsync(
		Guid userId,
		string currentPassword,
		string newPassword,
		CancellationToken cancellationToken = default)
	{
		if (userId == Guid.Empty)
		{
			throw new ValidationException(
				"User ID is required.");
		}

		if (string.IsNullOrWhiteSpace(
			currentPassword))
		{
			throw new ValidationException(
				"Current password is required.");
		}

		ValidatePasswordHash(newPassword);

		var user =
			await _userManager.FindByIdAsync(
				userId.ToString());

		if (user is null)
		{
			throw new NotFoundException(
				"User was not found.");
		}

		var result =
			await _userManager.ChangePasswordAsync(
				user,
				currentPassword,
				newPassword);

		if (!result.Succeeded)
		{
			throw CreateIdentityException(
				result,
				"Failed to change the password.");
		}

		user.MustChangePassword = false;

		var updateResult =
			await _userManager.UpdateAsync(user);

		if (!updateResult.Succeeded)
		{
			throw CreateIdentityException(
				updateResult,
				"Password was changed, but the user " +
				"account could not be updated.");
		}
	}

	private async Task<UserResponse> CreateUserAsync(
		string userName,
		string passwordHash,
		string role,
		Guid? memberId,
		bool mustChangePassword,
		CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(userName))
		{
			throw new ValidationException(
				"Username is required.");
		}

		ValidatePasswordHash(passwordHash);

		// ---------------------------------------------------------
		// Make sure the username isn't already being used.
		// ---------------------------------------------------------

		var existingUser =
			await _userManager.FindByNameAsync(
				userName);

		if (existingUser is not null)
		{
			throw new ConflictException(
				"A user account with this username " +
				"already exists.");
		}

		// ---------------------------------------------------------
		// Create the Identity user.
		// ---------------------------------------------------------

		var user = new ApplicationUser
		{
			UserName = userName,

			MemberId = memberId,

			MustChangePassword =
				mustChangePassword,

			PasswordHash = passwordHash
		};

		var createResult =
			await _userManager.CreateAsync(
				user);

		if (!createResult.Succeeded)
		{
			throw CreateIdentityException(
				createResult,
				"Failed to create the user account.");
		}

		// ---------------------------------------------------------
		// Assign the role.
		// ---------------------------------------------------------

		var roleResult =
			await _userManager.AddToRoleAsync(
				user,
				role);

		if (!roleResult.Succeeded)
		{
			// Don't leave an account without its required role.
			await _userManager.DeleteAsync(user);

			throw CreateIdentityException(
				roleResult,
				$"Failed to assign the {role} role " +
				"to the user account.");
		}

		return MapToResponse(user);
	}

	private static void ValidateStaffRequest(
		CreateStaffUserRequest request)
	{
		if (request is null)
		{
			throw new ValidationException(
				"User information is required.");
		}

		if (string.IsNullOrWhiteSpace(
			request.UserName))
		{
			throw new ValidationException(
				"Username is required.");
		}

		ValidatePasswordHash(request.Password);
	}

	private static void ValidatePasswordHash(
		string password)
	{
		if (string.IsNullOrWhiteSpace(password))
		{
			throw new ValidationException(
				"Password is required.");
		}
	}

	private static UserResponse MapToResponse(
		ApplicationUser user)
	{
		return new UserResponse
		{
			Id = user.Id,

			MemberId = user.MemberId,

			UserName =
				user.UserName ?? string.Empty,

			Email = user.Email
		};
	}

	private static ValidationException CreateIdentityException(
		IdentityResult result,
		string message)
	{
		var errors = string.Join(
			" ",
			result.Errors.Select(x =>
				$"{x.Code}: {x.Description}"));

		return new ValidationException(
			$"{message} {errors}");
	}
}