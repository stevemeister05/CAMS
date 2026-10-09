using CAMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Domain.Entities;

public class AttendanceQrSession : BaseEntity
{
	public Guid EventId { get; set; }

	public AttendanceAction Action { get; set; }

	public string Token { get; set; } = null!;

	public DateTime ExpiresAt { get; set; }

	public Event Event { get; set; } = null!;
}
