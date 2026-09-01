using CAMS.Application.Dashboard.DTOs;

namespace CAMS.Application.Dashboard;

public interface IDashboardRepository
{
	Task<AdminDashboardResponse> GetAdminDashboardAsync(
		DateOnly today,
		int trendDays,
		int recentAttendanceCount,
		int upcomingEventCount,
		CancellationToken cancellationToken = default);
}