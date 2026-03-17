using GymManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Infrastructure.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            // Table name (optional)
            builder.ToTable("Subscriptions");

            // Primary key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.AmountPaid)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.StartDate)
                .IsRequired();

            builder.Property(s => s.EndDate)
                .IsRequired();

            // Relationships
            builder.HasOne(s => s.Member)
                .WithMany()
                .HasForeignKey(s => s.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.MembershipPlan)
                .WithMany()
                .HasForeignKey(s => s.MembershipPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Query filter (soft delete)
            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}