using CAMS.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.MemberId)
			.IsRequired(false);

		builder.HasOne(x => x.Member)
			.WithOne()
			.HasForeignKey<ApplicationUser>(x => x.MemberId)
			.OnDelete(DeleteBehavior.SetNull);

		builder.HasIndex(x => x.MemberId)
			.IsUnique()
			.HasFilter("[MemberId] IS NOT NULL");
	}
}