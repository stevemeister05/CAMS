using CAMS.Application.Common;
using CAMS.Domain.Entities;
using CAMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Data;

public class CAMSDBContext
	: IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IUnitOfWork
{
	public CAMSDBContext(
		DbContextOptions<CAMSDBContext> options)
		: base(options)
	{
	}

	public DbSet<Member> Members => Set<Member>();

	public DbSet<RegistrationRequest> RegistrationRequests => Set<RegistrationRequest>();

	public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();

	public DbSet<Event> Events => Set<Event>();

	public DbSet<EventSchedule> EventSchedules => Set<EventSchedule>();

	public DbSet<Domain.Entities.Attendance> Attendances => Set<Domain.Entities.Attendance>();

	public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

	public DbSet<AttendanceQrSession> AttendanceQrSessions => Set<AttendanceQrSession>();

	public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(
		CancellationToken cancellationToken = default)
	{
		var transaction =
			await Database.BeginTransactionAsync(
				cancellationToken);

		return new EfCoreUnitOfWorkTransaction(
			transaction);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(CAMSDBContext).Assembly);
	}
}
