using CAMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
	public void Configure(EntityTypeBuilder<Member> builder)
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

		builder.HasIndex(x => x.MobileNumber)
			.IsUnique();

		builder.Property(x => x.Address)
			.HasMaxLength(500);

		builder.Property(x => x.Gender)
			.HasMaxLength(20);

		builder.Property(x => x.IsActive)
			.IsRequired();

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.HasMany(x => x.Attendances)
			.WithOne(x => x.Member)
			.HasForeignKey(x => x.MemberId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
