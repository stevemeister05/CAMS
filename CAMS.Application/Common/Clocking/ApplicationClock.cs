using CAMS.Application.Common.Clocking;

namespace CAMS.Infrastructure.Common.Clocking;

public sealed class ApplicationClock : IApplicationClock
{
	private static readonly TimeZoneInfo PhilippineTimeZone =
		TimeZoneInfo.FindSystemTimeZoneById(
			OperatingSystem.IsWindows()
				? "Singapore Standard Time"
				: "Asia/Manila");

	public DateTime UtcNow => DateTime.UtcNow;

	public DateTime LocalNow => TimeZoneInfo.ConvertTimeFromUtc(
		UtcNow,
		PhilippineTimeZone);
}
