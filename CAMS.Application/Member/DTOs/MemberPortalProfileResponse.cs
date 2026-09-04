namespace CAMS.Application.Member.DTOs;

public class MemberPortalProfileResponse
{
	public Guid Id { get; set; }

	public string FirstName { get; set; } =
		string.Empty;

	public string? MiddleName { get; set; }

	public string LastName { get; set; } =
		string.Empty;

	public string MobileNumber { get; set; } =
		string.Empty;

	public string? Address { get; set; }

	public DateOnly? BirthDate { get; set; }

	public string? Gender { get; set; }
}
