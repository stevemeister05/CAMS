using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class SystemSettingConfiguration
	: IEntityTypeConfiguration<SystemSetting>
{
	public void Configure(EntityTypeBuilder<SystemSetting> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Key)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(x => x.Value)
			.IsRequired()
			.HasMaxLength(2000);

		builder.Property(x => x.Description)
			.HasMaxLength(500);

		builder.Property(x => x.UpdatedAt)
			.IsRequired();

		builder.HasIndex(x => x.Key)
			.IsUnique();
	}
}
