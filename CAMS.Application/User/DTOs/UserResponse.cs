namespace CAMS.Application.User.DTOs;

public class UserResponse
{
	public Guid Id { get; set; }

	public Guid? MemberId { get; set; }

	public string UserName { get; set; } =
		string.Empty;

	public string? Email { get; set; }

	public string Role { get; set; } =
		string.Empty;

	public string? MemberName { get; set; }

	public bool MustChangePassword { get; set; }

	public bool IsMember =>
		MemberId.HasValue;

	public bool IsActive { get; set; }
}