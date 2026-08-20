using FluentAssertions;
using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace GymManagementSystem.Tests.Integrations.Caching
{
    public class MembershipPlanCachingTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public MembershipPlanCachingTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task UpdatePlan_ShouldInvalidateCache_AndReturnFreshDataOnNextGet()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var gym = await TestDataSeeder.SeedGymAsync(db);
            var plan = await TestDataSeeder.SeedMembershipPlanAsync(db, gym, price: 1000);

            // Act 1 — first GET populates the cache with price 1000
            var firstGet = await _client.GetAsync($"/api/plans/{plan.Id}");
            firstGet.EnsureSuccessStatusCode();
            var firstDto = await firstGet.Content.ReadFromJsonAsync<MembershipPlanDto>();
            firstDto!.Price.Should().Be(1000);

            // Act 2 — update the plan's price to 800
            var updatePayload = new UpdateMembershipPlanDto
            {
                Name = "Standard Plan", // must be non-empty — [Required]
                Price = 800,
                DurationInDays = 30
            };
            var updateResponse = await _client.PutAsJsonAsync($"/api/plans/{plan.Id}", updatePayload);
            updateResponse.EnsureSuccessStatusCode();

            // Act 3 — GET again, should reflect new price, not stale cached 1000
            var secondGet = await _client.GetAsync($"/api/plans/{plan.Id}");
            secondGet.EnsureSuccessStatusCode();
            var secondDto = await secondGet.Content.ReadFromJsonAsync<MembershipPlanDto>();

            // Assert — the actual invalidation check
            secondDto!.Price.Should().Be(800);
        }

        [Fact]
        public async Task GetAllPlans_SecondCall_ShouldReturnSameDataAsFirst()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var gym = await TestDataSeeder.SeedGymAsync(db);
            await TestDataSeeder.SeedMembershipPlanAsync(db, gym, price: 500);

            var first = await _client.GetFromJsonAsync<List<MembershipPlanDto>>("/api/plans");
            var second = await _client.GetFromJsonAsync<List<MembershipPlanDto>>("/api/plans");

            // Correctness check, not a timing check — integration tests shouldn't assert on wall-clock ms
            second.Should().BeEquivalentTo(first);
        }

        [Fact]
        public async Task DeletePlan_ShouldInvalidateCache_AndRemovePlanFromSubsequentGetAll()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var gym = await TestDataSeeder.SeedGymAsync(db);
            var plan = await TestDataSeeder.SeedMembershipPlanAsync(db, gym, price: 300);

            // Warm the cache
            await _client.GetAsync("/api/plans");

            var deleteResponse = await _client.DeleteAsync($"/api/plans/{plan.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            var afterDelete = await _client.GetFromJsonAsync<List<MembershipPlanDto>>("/api/plans");
            afterDelete!.Should().NotContain(p => p.Id == plan.Id);
        }
    }
}