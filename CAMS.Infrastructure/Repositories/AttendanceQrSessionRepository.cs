using CAMS.Application.Attendance;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Data.Repositories;

public class AttendanceQrSessionRepository
	: IAttendanceQrSessionRepository
{
	private readonly CAMSDBContext _context;

	public AttendanceQrSessionRepository(
		CAMSDBContext context)
	{
		_context = context;
	}

	public async Task<AttendanceQrSession?> GetByTokenAsync(
		string token,
		CancellationToken cancellationToken = default)
	{
		return await _context.AttendanceQrSessions
			.FirstOrDefaultAsync(
				x => x.Token == token,
				cancellationToken);
	}

	public async Task<AttendanceQrSession?> GetByEventAndActionAsync(
		Guid eventId,
		AttendanceAction action,
		CancellationToken cancellationToken = default)
	{
		return await _context.AttendanceQrSessions
			.FirstOrDefaultAsync(
				x =>
					x.EventId == eventId &&
					x.Action == action,
				cancellationToken);
	}

	public async Task AddAsync(
		AttendanceQrSession session,
		CancellationToken cancellationToken = default)
	{
		await _context.AttendanceQrSessions.AddAsync(
			session,
			cancellationToken);
	}

	public void Delete(AttendanceQrSession session)
	{
		_context.AttendanceQrSessions.Remove(
			session);
	}
}
