namespace CAMS.Domain.Entities;

public class Member : BaseEntity
{
	public Member() : base() { }

	public string FirstName { get; set; } = string.Empty;
	public string? MiddleName { get; set; }
	public string LastName { get; set; } = string.Empty;

	public string MobileNumber { get; set; } = string.Empty;

	public string? Address { get; set; }
	public DateOnly? BirthDate { get; set; }
	public string? Gender { get; set; }

	public bool IsActive { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public ICollection<Attendance> Attendances { get; set; } = [];
}
