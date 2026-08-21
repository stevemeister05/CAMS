using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.User.DTOs;

public class CreateUserRequest
{
	public string UserName { get; set; } = string.Empty;

	public string? Email { get; set; }

	public string Password { get; set; } = string.Empty;
}
