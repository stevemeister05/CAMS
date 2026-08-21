using CAMS.Application.User.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.User;

public interface IUserService
{
	Task<UserResponse> CreateMemberUserAsync(
		Guid memberId,
		string password,
		CancellationToken cancellationToken = default);

	Task<UserResponse> CreateAdministratorAsync(
		CreateStaffUserRequest request,
		CancellationToken cancellationToken = default);

	Task<UserResponse> CreateAttendanceStaffAsync(
		CreateStaffUserRequest request,
		CancellationToken cancellationToken = default);

	Task ChangePasswordAsync(
		Guid userId,
		string currentPassword,
		string newPassword,
		CancellationToken cancellationToken = default);
}
