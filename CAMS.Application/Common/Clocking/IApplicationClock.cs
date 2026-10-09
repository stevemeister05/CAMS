namespace CAMS.Application.Common.Clocking;

public interface IApplicationClock
{
	DateTime UtcNow { get; }

	DateTime LocalNow { get; }
}

