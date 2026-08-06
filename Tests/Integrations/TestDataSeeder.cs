using GymManagementSystem.Domain.Entities;
using GymManagementSystem.Infrastructure.Persistence;

namespace GymManagementSystem.Tests.Integrations;

public static class TestDataSeeder
{
    public static async Task<Gym> SeedGymAsync(
        ApplicationDbContext db,
        int capacity = 100)
    {
        var gym = new Gym
        {
            Name = "Fitness Center",
            Address = "Cairo",
            PhoneNumber = "01000000000",
            Capacity = capacity
        };

        db.Gyms.Add(gym);
        await db.SaveChangesAsync();

        return gym;
    }

    public static async Task<Member> SeedMemberAsync(
        ApplicationDbContext db,
        Gym gym,
        string email = "john@gmail.com")
    {
        var member = new Member
        {
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1995, 1, 1),
            JoinDate = DateTime.UtcNow,
            GymId = gym.Id
        };

        db.Members.Add(member);
        await db.SaveChangesAsync();

        return member;
    }
}