using CAMS.Domain.Entities;
using CAMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Infrastructure.Data;

public class CAMSDBContext
	: IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
	public CAMSDBContext(
		DbContextOptions<CAMSDBContext> options)
		: base(options)
	{
	}

	public DbSet<Member> Members => Set<Member>();

	public DbSet<RegistrationRequest> RegistrationRequests =>
		Set<RegistrationRequest>();

	public DbSet<OtpVerification> OtpVerifications =>
		Set<OtpVerification>();

	public DbSet<Event> Events => Set<Event>();

	public DbSet<EventSchedule> EventSchedules =>
		Set<EventSchedule>();

	public DbSet<Attendance> Attendances =>
		Set<Attendance>();

	public DbSet<SystemSetting> SystemSettings =>
		Set<SystemSetting>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(CAMSDBContext).Assembly);
	}
}
