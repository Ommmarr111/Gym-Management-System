using GymManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Infrastructure.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("Members");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(m => m.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(m => m.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(m => m.Email)
                .IsUnique(); // ← Email must be unique

            builder.Property(m => m.PhoneNumber)
                .HasMaxLength(15)
                .IsRequired();

            builder.HasOne(m => m.Gym)
                .WithMany()
                .HasForeignKey(m => m.GymId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}