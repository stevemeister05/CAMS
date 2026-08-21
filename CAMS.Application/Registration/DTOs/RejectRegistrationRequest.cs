using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CAMS.Application.Registration.DTOs;

public class RejectRegistrationRequest
{
	[Required]
	public string Reason { get; set; } = string.Empty;
}