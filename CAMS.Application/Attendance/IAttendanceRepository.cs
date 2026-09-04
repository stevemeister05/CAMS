namespace CAMS.Application.Attendance;

public interface IAttendanceRepository
{
	Task<Domain.Entities.Attendance?> GetByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Domain.Entities.Attendance>> GetByEventAsync(
		Guid eventId,
		CancellationToken cancellationToken = default);

	Task<Domain.Entities.Attendance?> GetByMemberAndEventAsync(
		Guid memberId,
		Guid eventId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Domain.Entities.Attendance>> GetByMemberAsync(
		Guid memberId,
		CancellationToken cancellationToken = default);

	Task AddAsync(
		Domain.Entities.Attendance attendance,
		CancellationToken cancellationToken = default);

	void Update(
		Domain.Entities.Attendance attendance);

	void Delete(
		Domain.Entities.Attendance attendance);
}