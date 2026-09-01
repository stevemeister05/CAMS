using CAMS.Domain.Enums;

namespace CAMS.Web.Realtime;

public sealed record AttendanceQrTarget(
	Guid EventId,
	AttendanceAction Action);