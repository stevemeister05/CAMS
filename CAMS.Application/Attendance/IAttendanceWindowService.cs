using CAMS.Domain.Enums;

namespace CAMS.Application.Attendance;

public interface IAttendanceWindowService
{
	bool IsOpen(
		Domain.Entities.Event @event,
		AttendanceAction action,
		DateTime localNow);

	void Validate(
		Domain.Entities.Event @event,
		AttendanceAction action,
		DateTime localNow);
}