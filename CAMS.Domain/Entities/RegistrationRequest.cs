using CAMS.Domain.Enums;

namespace CAMS.Domain.Entities;

public class RegistrationRequest : BaseEntity
{
	public RegistrationRequest() : base() { }

	public string FirstName { get; set; } = string.Empty;
	public string? MiddleName { get; set; }
	public string LastName { get; set; } = string.Empty;

	public string MobileNumber { get; set; } = string.Empty;

	public string? Address { get; set; }
	public DateOnly? BirthDate { get; set; }
	public string? Gender { get; set; }

	public RegistrationStatus Status { get; set; }

	public DateTime RegisteredAt { get; set; }
	public DateTime? ApprovedAt { get; set; }
	public DateTime? RejectedAt { get; set; }

	public string? RejectionReason { get; set; }
}
