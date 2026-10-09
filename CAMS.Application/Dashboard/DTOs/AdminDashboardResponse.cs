namespace CAMS.Application.Dashboard.DTOs;

public sealed class AdminDashboardResponse
{
	public DateOnly Date { get; set; }

	public int ActiveMemberCount { get; set; }

	public int TodayAttendanceCount { get; set; }

	public decimal TodayAttendanceRate { get; set; }

	public int PendingRegistrationCount { get; set; }

	public TodayEventSummaryResponse TodayEventSummary { get; set; } =
		new();

	public IReadOnlyList<DashboardEventResponse> TodayEvents { get; set; } =
		[];

	public IReadOnlyList<AttendanceTrendPointResponse> AttendanceTrend { get; set; } =
		[];

	public IReadOnlyList<RecentAttendanceResponse> RecentAttendances { get; set; } =
		[];

	public IReadOnlyList<DashboardEventResponse> UpcomingEvents { get; set; } =
		[];
}


public sealed class TodayEventSummaryResponse
{
	public int Total { get; set; }

	public int Scheduled { get; set; }

	public int Ongoing { get; set; }

	public int Completed { get; set; }
}


public sealed class DashboardEventResponse
{
	public Guid Id { get; set; }

	public string Name { get; set; } =
		string.Empty;

	public DateOnly EventDate { get; set; }

	public TimeOnly StartTime { get; set; }

	public TimeOnly EndTime { get; set; }

	public TimeOnly AttendanceTimeInStart { get; set; }

	public TimeOnly AttendanceTimeInEnd { get; set; }

	public TimeOnly AttendanceTimeOutStart { get; set; }

	public TimeOnly AttendanceTimeOutEnd { get; set; }

	public string Type { get; set; } =
		string.Empty;

	public string Status { get; set; } =
		string.Empty;

	public int AttendanceCount { get; set; }
}


public sealed class AttendanceTrendPointResponse
{
	public DateOnly Date { get; set; }

	public int AttendanceCount { get; set; }
}


public sealed class RecentAttendanceResponse
{
	public Guid Id { get; set; }

	public Guid MemberId { get; set; }

	public string MemberName { get; set; } =
		string.Empty;

	public Guid EventId { get; set; }

	public string EventName { get; set; } =
		string.Empty;

	public DateTime TimeIn { get; set; }

	public DateTime? TimeOut { get; set; }
}