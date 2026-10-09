using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Domain.Constants;

public static class ApplicationRoles
{
	public const string Administrator = "Administrator";

	public const string AttendanceStaff = "AttendanceStaff";

	public const string Member = "Member";

	public const string AdministratorOrAttendanceStaff =
		Administrator + "," + AttendanceStaff;

	public static readonly IReadOnlyList<string> All =
	[
		Administrator,
		Member,
		AttendanceStaff
	];
}
