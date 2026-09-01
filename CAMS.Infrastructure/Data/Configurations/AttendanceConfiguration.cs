using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class AttendanceConfiguration
	: IEntityTypeConfiguration<Domain.Entities.Attendance>
{
	public void Configure(
		EntityTypeBuilder<Domain.Entities.Attendance> builder)
	{
		builder.HasKey(x => x.Id);

		// ---------------------------------------------------------
		// Member
		// ---------------------------------------------------------

		builder.Property(x => x.MemberId)
			.IsRequired();

		builder.HasOne(x => x.Member)
			.WithMany()
			.HasForeignKey(x => x.MemberId)
			.OnDelete(DeleteBehavior.Restrict);

		// ---------------------------------------------------------
		// Event
		// ---------------------------------------------------------

		builder.Property(x => x.EventId)
			.IsRequired();

		builder.HasOne(x => x.Event)
			.WithMany()
			.HasForeignKey(x => x.EventId)
			.OnDelete(DeleteBehavior.Restrict);

		// ---------------------------------------------------------
		// Time-In
		// ---------------------------------------------------------

		builder.Property(x => x.TimeIn)
			.IsRequired();

		builder.Property(x => x.TimeInMethod)
			.IsRequired();

		// ---------------------------------------------------------
		// Time-Out
		// ---------------------------------------------------------

		builder.Property(x => x.TimeOut)
			.IsRequired(false);

		builder.Property(x => x.TimeOutMethod)
			.IsRequired(false);

		// ---------------------------------------------------------
		// UpdatedAt
		// ---------------------------------------------------------

		builder.Property(x => x.UpdatedAt)
			.IsRequired(false);

		// ---------------------------------------------------------
		// IMPORTANT:
		// One attendance record per member per event.
		// ---------------------------------------------------------

		builder.HasIndex(x => new
		{
			x.MemberId,
			x.EventId
		})
		.IsUnique();
	}
}
