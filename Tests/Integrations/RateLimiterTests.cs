using GymManagementSystem.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace GymManagementSystem.Tests.Integrations
{
    public class RateLimiterTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RateLimiterTests(WebApplicationFactory<Program> factory)
        {
            // Creates a virtual client that communicates directly with your API's pipeline in memory
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task LoginEndpoint_ShouldReturn429_WhenLimitExceeded()
        {
            // Arrange
            var url = "/api/auth/login";
            var permitLimit = 5; // The maximum requests defined in the "StrictAuth" policy
            var dummyPayload = new { Username = "testuser", Password = "password123" };

            // Act & Assert 1: Exhaust the allowed quota
            for (int i = 0; i < permitLimit; i++)
            {
                var response = await _client.PostAsJsonAsync(url, dummyPayload);

                // The first 5 requests should execute normally (e.g., 200 OK or 401 Unauthorized)
                // The key is that they are NOT blocked by the rate limiter.
                Assert.NotEqual(HttpStatusCode.TooManyRequests, response.StatusCode);
            }

            // Act 2: Fire the one request that breaks the limit
            var blockedResponse = await _client.PostAsJsonAsync(url, dummyPayload);

            // Assert 2: The middleware should intercept this and return 429 immediately
            Assert.Equal(HttpStatusCode.TooManyRequests, blockedResponse.StatusCode);
        }
    }

}
