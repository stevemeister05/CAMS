using CAMS.Application.Attendance;
using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Data.Repositories;

public class AttendanceRepository
	: IAttendanceRepository
{
	private readonly CAMSDBContext _context;

	public AttendanceRepository(
		CAMSDBContext context)
	{
		_context = context;
	}

	public async Task<Domain.Entities.Attendance?> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default)
	{
		return await _context.Attendances
			.FirstOrDefaultAsync(
				x => x.Id == id,
				cancellationToken);
	}

	public async Task<IReadOnlyList<Attendance>> GetByEventAsync(
		Guid eventId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Attendances
			.AsNoTracking()
			.Include(x => x.Member)
			.Where(x => x.EventId == eventId)
			.OrderBy(x => x.TimeIn)
			.ToListAsync(cancellationToken);
	}

	public async Task<Domain.Entities.Attendance?> GetByMemberAndEventAsync(
		Guid memberId,
		Guid eventId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Attendances
			.FirstOrDefaultAsync(
				x =>
					x.MemberId == memberId &&
					x.EventId == eventId,
				cancellationToken);
	}

	public async Task<IReadOnlyList<Domain.Entities.Attendance>> GetByMemberAsync(
		Guid memberId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Attendances
			.AsNoTracking()
			.Include(x =>
				x.Event)
			.Where(x =>
				x.MemberId == memberId)
			.OrderByDescending(x =>
				x.Event.EventDate)
			.ThenByDescending(x =>
				x.TimeIn)
			.ToListAsync(
				cancellationToken);
	}

	public async Task AddAsync(
		Domain.Entities.Attendance attendance,
		CancellationToken cancellationToken = default)
	{
		await _context.Attendances.AddAsync(
			attendance,
			cancellationToken);
	}

	public void Update(
		Domain.Entities.Attendance attendance)
	{
		_context.Attendances.Update(
			attendance);
	}

	public void Delete(
		Domain.Entities.Attendance attendance)
	{
		_context.Attendances.Remove(
			attendance);
	}
}