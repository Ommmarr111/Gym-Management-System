using GymManagementSystem.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace GymManagementSystem.Tests.Integrations
{
    public class MemberIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MemberIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/members");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
