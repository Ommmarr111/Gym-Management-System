using GymManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Gym> Gyms { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Subscription>(entity =>
            {
                entity.Property(s => s.AmountPaid).HasColumnType("decimal(18,2)");
                entity.HasOne(s => s.Member)
                      .WithMany()
                      .HasForeignKey(s => s.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<MembershipPlan>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });
            builder.Entity<Attendance>(entity =>
            {
                entity.HasOne(a => a.Member)
                      .WithMany()
                      .HasForeignKey(a => a.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Gym)
                      .WithMany()
                      .HasForeignKey(a => a.GymId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Gym>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<MembershipPlan>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Subscription>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<WorkoutPlan>().HasQueryFilter(x => !x.IsDeleted);
            builder.Entity<Exercise>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
