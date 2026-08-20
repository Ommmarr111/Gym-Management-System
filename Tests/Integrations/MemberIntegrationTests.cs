using GymManagementSystem.Application.DTOs;
using GymManagementSystem.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace GymManagementSystem.Tests.Integrations;

public class MemberIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MemberIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    private async Task ExecuteWithDbContextAsync(Func<ApplicationDbContext, Task> action)
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        await action(db);
    }

    [Fact]
    public async Task GetAll_Should_Return_Empty_List_When_NoMembersExist()
    {
        // Arrange
        await ResetDatabaseAsync();
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/api/members");
        response.EnsureSuccessStatusCode();
        var result = await response.Content
            .ReadFromJsonAsync<PagedResult<MemberDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetAll_Should_Return_All_Members()
    {
        // Arrange
        await ResetDatabaseAsync();
        var client = CreateClient();
        await ExecuteWithDbContextAsync(async db =>
        {
            var gym = await TestDataSeeder.SeedGymAsync(db);
            await TestDataSeeder.SeedMemberAsync(db, gym);
        });

        // Act
        var response = await client.GetAsync("/api/members");
        response.EnsureSuccessStatusCode();
        var result = await response.Content
            .ReadFromJsonAsync<PagedResult<MemberDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("John Doe", result.Items[0].FullName);
        Assert.Equal("Fitness Center", result.Items[0].GymName);
    }
}