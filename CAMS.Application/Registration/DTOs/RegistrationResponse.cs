using CAMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Registration.DTOs;

public class RegistrationResponse
{
	public Guid RegistrationId { get; set; }

	public string MobileNumber { get; set; } = string.Empty;

	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string? MiddleName { get; set; }

	public RegistrationStatus Status { get; set; }

	public DateTime RegisteredAt { get; set; }
}
