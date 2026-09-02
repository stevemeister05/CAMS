using CAMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
	public Guid? MemberId { get; set; }

	public bool MustChangePassword { get; set; } = false;

	public bool IsActive { get; set; } = true;

	public Member? Member { get; set; }
}
