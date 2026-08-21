using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.User.DTOs;

public class UserResponse
{
	public Guid Id { get; set; }

	public Guid? MemberId { get; set; }

	public string UserName { get; set; } = string.Empty;

	public string? Email { get; set; }
}
