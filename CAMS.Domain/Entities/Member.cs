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

	public ICollection<MemberFingerprint> Fingerprints
	{
		get;
		set;
	} = [];

	/// <summary>
	/// Gets the member's full name in the format:
	/// FirstName M. LastName
	/// </summary>
	public string GetFullName()
	{
		var middleInitial = GetMiddleInitial();

		return string.IsNullOrWhiteSpace(middleInitial)
			? $"{FirstName} {LastName}".Trim()
			: $"{FirstName} {middleInitial}. {LastName}".Trim();
	}

	/// <summary>
	/// Gets the member's full name in the format:
	/// LastName, FirstName M.
	/// </summary>
	public string GetFullNameLastFirst()
	{
		var middleInitial = GetMiddleInitial();

		return string.IsNullOrWhiteSpace(middleInitial)
			? $"{LastName}, {FirstName}".Trim()
			: $"{LastName}, {FirstName} {middleInitial}.".Trim();
	}

	private string? GetMiddleInitial()
	{
		if (string.IsNullOrWhiteSpace(MiddleName))
			return null;

		return MiddleName.Trim()[0].ToString().ToUpperInvariant();
	}
}
