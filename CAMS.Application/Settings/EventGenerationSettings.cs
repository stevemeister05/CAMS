namespace CAMS.Application.Settings;

/// <summary>
/// Configuration for automatic event generation.
/// </summary>
/// <remarks>
/// Example:
///
/// "EventGeneration": {
///   "GenerateDaysAhead": 30,
///   "Schedule": {
///     "Type": "Daily",
///     "Hour": 0,
///     "Minute": 5,
///     "Second": 0
///   }
/// }
///
/// GenerateDaysAhead determines how far into the future the system
/// should generate events. For example, a value of 30 causes the
/// generator to maintain events for the next 30 days.
/// </remarks>
public class EventGenerationSettings
{
	/// <summary>
	/// Gets or sets the number of days into the future for which
	/// events should be generated.
	/// </summary>
	public int GenerateDaysAhead { get; set; } = 30;

	/// <summary>
	/// Gets or sets the schedule used to determine when automatic
	/// event generation runs.
	/// </summary>
	public ScheduleSettings Schedule { get; set; } = new();
}
