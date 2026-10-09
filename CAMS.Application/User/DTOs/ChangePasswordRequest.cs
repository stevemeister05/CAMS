using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.User.DTOs;

public class ChangePasswordRequest
{
	public string CurrentPassword { get; set; } = string.Empty;

	public string NewPassword { get; set; } = string.Empty;
}
