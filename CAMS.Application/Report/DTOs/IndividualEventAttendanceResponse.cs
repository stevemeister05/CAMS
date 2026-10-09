using CAMS.Domain.Enums;

namespace CAMS.Application.Report.DTOs;

public sealed class IndividualEventAttendanceResponse
{
	public Guid MemberId { get; set; }

	public string MemberName { get; set; } =
		string.Empty;

	public DateTime? TimeIn { get; set; }

	public DateTime? TimeOut { get; set; }

	public AttendanceMethod? TimeInMethod { get; set; }

	public AttendanceMethod? TimeOutMethod { get; set; }
}