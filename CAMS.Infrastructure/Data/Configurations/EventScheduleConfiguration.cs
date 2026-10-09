using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Infrastructure.Data.Configurations;

public class EventScheduleConfiguration
	: IEntityTypeConfiguration<EventSchedule>
{
	public void Configure(EntityTypeBuilder<EventSchedule> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(x => x.EventType)
			.IsRequired();

		builder.Property(x => x.IsActive)
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

		builder.Property(x => x.DayOfWeek)
			.IsRequired(false);

		builder.Property(x => x.StartDay)
			.IsRequired(false);

		builder.Property(x => x.StartMonth)
			.IsRequired(false);

		builder.Property(x => x.EndDay)
			.IsRequired(false);

		builder.Property(x => x.EndMonth)
			.IsRequired(false);

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.HasIndex(x => new
		{
			x.EventType,
			x.IsActive
		});
	}
}
