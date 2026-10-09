using CAMS.Application.Dashboard.DTOs;

namespace CAMS.Application.Dashboard;

public interface IDashboardService
{
	Task<AdminDashboardResponse> GetAdminDashboardAsync(
		CancellationToken cancellationToken = default);
}