using CAMS.Domain.Enums;

namespace CAMS.Domain.Entities;

public class Attendance : BaseEntity
{
	public Guid MemberId { get; set; }
	public Guid EventId { get; set; }

	public DateTime? TimeIn { get; set; }
	public DateTime? TimeOut { get; set; }

	public AttendanceMethod TimeInMethod { get; set; }
	public AttendanceMethod? TimeOutMethod { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public Member Member { get; set; } = null!;
	public Event Event { get; set; } = null!;
}
