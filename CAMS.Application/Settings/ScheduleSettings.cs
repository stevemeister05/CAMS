using CAMS.Domain.Enums;

namespace CAMS.Application.Settings;

/// <summary>
/// Defines when a scheduled operation should execute.
/// 
/// The properties required depend on the configured <see cref="ScheduleType"/>.
/// Properties that are not applicable to the selected schedule type should be left null.
/// </summary>
/// <remarks>
/// Configuration examples:
///
/// Yearly:
/// {
///   "Type": "Yearly",
///   "Month": 12,
///   "Day": 25,
///   "Hour": 0,
///   "Minute": 0,
///   "Second": 0
/// }
///
/// Monthly:
/// {
///   "Type": "Monthly",
///   "Day": 1,
///   "Hour": 0,
///   "Minute": 0,
///   "Second": 0
/// }
///
/// Weekly:
/// {
///   "Type": "Weekly",
///   "DayOfWeek": "Sunday",
///   "Hour": 0,
///   "Minute": 0,
///   "Second": 0
/// }
///
/// Daily:
/// {
///   "Type": "Daily",
///   "Hour": 0,
///   "Minute": 5,
///   "Second": 0
/// }
///
/// Hourly:
/// {
///   "Type": "Hourly",
///   "Minute": 5,
///   "Second": 0
/// }
///
/// Minutely:
/// {
///   "Type": "Minutely",
///   "Second": 30
/// }
///
/// Interval:
/// {
///   "Type": "Interval",
///   "IntervalSeconds": 300
/// }
/// </remarks>
public class ScheduleSettings
{
	/// <summary>
	/// Gets or sets the type of schedule that determines which
	/// date/time properties are used.
	/// </summary>
	public ScheduleType Type { get; set; }

	/// <summary>
	/// Gets or sets the month on which the schedule executes.
	/// Required for <see cref="ScheduleType.Yearly"/>.
	/// Valid values are 1 through 12.
	/// </summary>
	public int? Month { get; set; }

	/// <summary>
	/// Gets or sets the day of the month on which the schedule executes.
	/// Required for <see cref="ScheduleType.Yearly"/> and
	/// <see cref="ScheduleType.Monthly"/>.
	/// Valid values are 1 through 31.
	/// </summary>
	public int? Day { get; set; }

	/// <summary>
	/// Gets or sets the day of the week on which the schedule executes.
	/// Required for <see cref="ScheduleType.Weekly"/>.
	/// </summary>
	public DayOfWeek? DayOfWeek { get; set; }

	/// <summary>
	/// Gets or sets the hour at which the schedule executes.
	/// Required for <see cref="ScheduleType.Yearly"/>,
	/// <see cref="ScheduleType.Monthly"/>,
	/// <see cref="ScheduleType.Weekly"/>, and
	/// <see cref="ScheduleType.Daily"/>.
	/// Valid values are 0 through 23.
	/// </summary>
	public int? Hour { get; set; }

	/// <summary>
	/// Gets or sets the minute at which the schedule executes.
	/// Required for <see cref="ScheduleType.Yearly"/>,
	/// <see cref="ScheduleType.Monthly"/>,
	/// <see cref="ScheduleType.Weekly"/>,
	/// <see cref="ScheduleType.Daily"/>, and
	/// <see cref="ScheduleType.Hourly"/>.
	/// Valid values are 0 through 59.
	/// </summary>
	public int? Minute { get; set; }

	/// <summary>
	/// Gets or sets the second at which the schedule executes.
	/// Required for <see cref="ScheduleType.Yearly"/>,
	/// <see cref="ScheduleType.Monthly"/>,
	/// <see cref="ScheduleType.Weekly"/>,
	/// <see cref="ScheduleType.Daily"/>,
	/// <see cref="ScheduleType.Hourly"/>, and
	/// <see cref="ScheduleType.Minutely"/>.
	/// Valid values are 0 through 59.
	/// </summary>
	public int? Second { get; set; }

	/// <summary>
	/// Gets or sets the interval, in seconds, between executions.
	/// Required only for <see cref="ScheduleType.Interval"/>.
	/// Must be greater than zero.
	/// </summary>
	public int? IntervalSeconds { get; set; }
}