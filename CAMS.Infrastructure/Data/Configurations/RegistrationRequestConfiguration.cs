using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class RegistrationRequestConfiguration
	: IEntityTypeConfiguration<RegistrationRequest>
{
	public void Configure(EntityTypeBuilder<RegistrationRequest> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.FirstName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(x => x.MiddleName)
			.HasMaxLength(100);

		builder.Property(x => x.LastName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(x => x.MobileNumber)
			.IsRequired()
			.HasMaxLength(20);

		builder.Property(x => x.Address)
			.HasMaxLength(500);

		builder.Property(x => x.Gender)
			.HasMaxLength(20);

		builder.Property(x => x.Status)
			.IsRequired();

		builder.Property(x => x.RegisteredAt)
			.IsRequired();

		builder.Property(x => x.RejectionReason)
			.HasMaxLength(500);

		builder.HasIndex(x => new
		{
			x.MobileNumber,
			x.Status
		});
	}
}
