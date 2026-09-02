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

	Task<IReadOnlyList<UserResponse>> GetAllAsync(
	CancellationToken cancellationToken = default);

	Task<UserResponse> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<UserResponse> UpdateStaffUserAsync(
		Guid id,
		UpdateStaffUserRequest request,
		CancellationToken cancellationToken = default);

	Task ResetPasswordAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task SetActiveStatusAsync(
		Guid id,
		bool isActive,
		CancellationToken cancellationToken = default);

	Task<bool> IsUserActiveAsync(
		Guid userId,
		CancellationToken cancellationToken = default);
}
