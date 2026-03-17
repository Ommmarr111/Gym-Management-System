using GymManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Infrastructure.Configurations
{
    public class GymConfiguration : IEntityTypeConfiguration<Gym>
    {
        public void Configure(EntityTypeBuilder<Gym> builder)
        {
            builder.ToTable("Gyms");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(g => g.Address)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(g => g.PhoneNumber)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(g => g.Capacity)
                .IsRequired();

            builder.HasQueryFilter(g => !g.IsDeleted);
        }
    }
}