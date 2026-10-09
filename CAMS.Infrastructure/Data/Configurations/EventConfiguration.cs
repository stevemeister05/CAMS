using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
	public void Configure(EntityTypeBuilder<Event> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(x => x.Description)
			.HasMaxLength(1000);

		builder.Property(x => x.Type)
			.IsRequired();

		builder.Property(x => x.EventDate)
			.IsRequired();

		builder.Property(x => x.StartTime)
			.IsRequired();

		builder.Property(x => x.EndTime)
			.IsRequired();

		builder.Property(x => x.AttendanceTimeInStart)
			.IsRequired();

		builder.Property(x => x.AttendanceTimeInEnd)
			.IsRequired();

		builder.Property(x => x.AttendanceTimeOutStart)
			.IsRequired();

		builder.Property(x => x.AttendanceTimeOutEnd)
			.IsRequired();

		builder.Property(x => x.Status)
			.IsRequired();

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.HasIndex(x => new
		{
			x.EventDate,
			x.Type
		});

		builder.HasMany(x => x.Attendances)
			.WithOne(x => x.Event)
			.HasForeignKey(x => x.EventId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
