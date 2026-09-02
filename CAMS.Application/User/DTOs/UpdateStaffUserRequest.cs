namespace CAMS.Application.User.DTOs;

public class UpdateStaffUserRequest
{
	public string UserName { get; set; } = string.Empty;
}

public class UpdateUserStatusRequest
{
	public bool IsActive { get; set; }
}