using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class OtpVerificationConfiguration
	: IEntityTypeConfiguration<OtpVerification>
{
	public void Configure(EntityTypeBuilder<OtpVerification> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.MobileNumber)
			.IsRequired()
			.HasMaxLength(20);

		builder.Property(x => x.OtpCode)
			.IsRequired()
			.HasMaxLength(128);

		builder.Property(x => x.Purpose)
			.IsRequired();

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.Property(x => x.ExpiresAt)
			.IsRequired();

		builder.Property(x => x.AttemptCount)
			.IsRequired();

		builder.Property(x => x.IsVerified)
			.IsRequired();

		builder.HasIndex(x => new
		{
			x.MobileNumber,
			x.Purpose,
			x.CreatedAt
		});
	}
}
