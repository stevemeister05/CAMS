namespace CAMS.Web.Extensions;

public static class DateTimeExtensions
{
	private static readonly TimeZoneInfo PhilippineTimeZone =
		TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

	public static DateTime ToPhilippineTime(this DateTime utcDateTime)
	{
		return TimeZoneInfo.ConvertTimeFromUtc(
			DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc),
			PhilippineTimeZone);
	}

	public static DateTime? ToPhilippineTime(this DateTime? utcDateTime)
	{
		return utcDateTime?.ToPhilippineTime();
	}
}