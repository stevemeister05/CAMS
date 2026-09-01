using CAMS.Application.Common.Clocking;
using CAMS.Application.Dashboard.DTOs;

namespace CAMS.Application.Dashboard;

public sealed class DashboardService : IDashboardService
{
	private readonly IDashboardRepository _dashboardRepository;
	private readonly IApplicationClock _clock;


	public DashboardService(
		IDashboardRepository dashboardRepository,
		IApplicationClock clock)
	{
		_dashboardRepository =
			dashboardRepository;

		_clock =
			clock;
	}


	public async Task<AdminDashboardResponse>
		GetAdminDashboardAsync(
			CancellationToken cancellationToken = default)
	{
		var today =
			DateOnly.FromDateTime(
				_clock.LocalNow);


		return await _dashboardRepository
			.GetAdminDashboardAsync(
				today: today,
				trendDays: 7,
				recentAttendanceCount: 10,
				upcomingEventCount: 5,
				cancellationToken:
					cancellationToken);
	}
}