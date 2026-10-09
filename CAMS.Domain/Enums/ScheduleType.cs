namespace CAMS.Domain.Enums;

/// <summary>
/// Defines how frequently or when a scheduled operation should execute.
/// </summary>
public enum ScheduleType
{
	/// <summary>
	/// Executes once every year at the specified month, day, hour, minute, and second.
	/// </summary>
	Yearly = 1,

	/// <summary>
	/// Executes once every month on the specified day, hour, minute, and second.
	/// </summary>
	Monthly = 2,

	/// <summary>
	/// Executes once every week on the specified day of the week, hour, minute, and second.
	/// </summary>
	Weekly = 3,

	/// <summary>
	/// Executes once every day at the specified hour, minute, and second.
	/// </summary>
	Daily = 4,

	/// <summary>
	/// Executes once every hour at the specified minute and second.
	/// </summary>
	Hourly = 5,

	/// <summary>
	/// Executes once every minute at the specified second.
	/// </summary>
	Minutely = 6,

	/// <summary>
	/// Executes repeatedly using a fixed interval specified in seconds.
	/// </summary>
	Interval = 7
}