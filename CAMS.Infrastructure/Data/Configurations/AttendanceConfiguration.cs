using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Infrastructure.Data.Configurations;

public class AttendanceConfiguration
	: IEntityTypeConfiguration<Attendance>
{
	public void Configure(EntityTypeBuilder<Attendance> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.TimeIn)
			.IsRequired(false);

		builder.Property(x => x.TimeOut)
			.IsRequired(false);

		builder.Property(x => x.TimeInMethod)
			.IsRequired();

		builder.Property(x => x.TimeOutMethod)
			.IsRequired(false);

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.HasOne(x => x.Member)
			.WithMany(x => x.Attendances)
			.HasForeignKey(x => x.MemberId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(x => x.Event)
			.WithMany(x => x.Attendances)
			.HasForeignKey(x => x.EventId)
			.OnDelete(DeleteBehavior.Restrict);

		// One attendance record per member per event
		builder.HasIndex(x => new
		{
			x.MemberId,
			x.EventId
		})
		.IsUnique();

		builder.HasIndex(x => x.EventId);
		builder.HasIndex(x => x.MemberId);
	}
}
