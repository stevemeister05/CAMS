using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CAMS.Application.Registration.DTOs;

public class RegisterRequest
{
	[Required]
	[MaxLength(100)]
	public string FirstName { get; set; } = string.Empty;

	[MaxLength(100)]
	public string? MiddleName { get; set; }

	[Required]
	[MaxLength(100)]
	public string LastName { get; set; } = string.Empty;

	[Required]
	public string MobileNumber { get; set; } = string.Empty;

	[Required]
	public string Password { get; set; } = string.Empty;

	public string? Address { get; set; }

	public DateOnly? BirthDate { get; set; }

	public string? Gender { get; set; }
}
