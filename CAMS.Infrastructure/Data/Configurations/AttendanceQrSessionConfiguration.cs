using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class AttendanceQrSessionConfiguration
	: IEntityTypeConfiguration<AttendanceQrSession>
{
	public void Configure(EntityTypeBuilder<AttendanceQrSession> builder)
	{
		builder.HasKey(x => x.Id);

		builder
			.Property(x => x.Action)
			.IsRequired();
		
		builder
			.Property(x => x.Token)
			.IsRequired()
			.HasMaxLength(128);

		builder
			.Property(x => x.CreatedAt)
			.IsRequired();

		builder
			.Property(x => x.ExpiresAt)
			.IsRequired();

		/* * Every QR token must be unique. */
		builder
			.HasIndex(x => x.Token)
			.IsUnique();

		/* * Only one QR session may exist for a specific * event and attendance action. 
		 * * Examples: 
		 * * Event 1 + TimeIn -> one session * Event 1 + TimeOut -> one session * Event 2 + TimeIn -> one session * Event 2 + TimeOut -> one session */
		builder
			.HasIndex(x => new { x.EventId, x.Action })
			.IsUnique();

		/* * Relationship: * * Event * └── AttendanceQrSession */
		builder
			.HasOne(x => x.Event)
			.WithMany()
			.HasForeignKey(x => x.EventId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}