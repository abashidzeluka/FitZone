using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FitZone.Tests
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AssignRole_WithoutAuthHeader_Returns401Unauthorized()
        {
            var response = await _client.PostAsync("/api/Auth/assign-role/1/Trainer", null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AssignRole_AsMember_Returns403Forbidden()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/assign-role/2/Trainer");
            request.Headers.Add("X-Test-UserId", "1");
            request.Headers.Add("X-Test-Role", "Member");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AssignRole_AdminTryingToAssignAdminRole_Returns403Forbidden()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/assign-role/2/Admin");
            request.Headers.Add("X-Test-UserId", "1");
            request.Headers.Add("X-Test-Role", "Admin");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task RemoveRole_AdminTryingToRemoveAdminRole_Returns403Forbidden()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/Auth/remove-role/2/Admin");
            request.Headers.Add("X-Test-UserId", "1");
            request.Headers.Add("X-Test-Role", "Admin");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AssignRole_SuperAdminAssigningRoleToNonExistentUser_Returns404NotFound()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/assign-role/99999/Admin");
            request.Headers.Add("X-Test-UserId", "1");
            request.Headers.Add("X-Test-Role", "superAdmin");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}