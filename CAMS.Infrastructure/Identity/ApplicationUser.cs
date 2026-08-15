using CAMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
	public Guid? MemberId { get; set; }

	public Member? Member { get; set; }
}
