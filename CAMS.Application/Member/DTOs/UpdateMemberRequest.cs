using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Member.DTOs;

public sealed class UpdateMemberRequest
{
	public string FirstName { get; set; } = string.Empty;

	public string? MiddleName { get; set; }

	public string LastName { get; set; } = string.Empty;

	public string MobileNumber { get; set; } = string.Empty;

	public DateOnly? BirthDate { get; set; }

	public string? Gender { get; set; }

	public string? Address { get; set; }
}
