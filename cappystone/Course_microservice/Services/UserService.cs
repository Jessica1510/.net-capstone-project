using System.Net.Http;
using System.Threading.Tasks;

namespace Course_microservice.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsUserValid(string userId)
        {
            var response = await _httpClient.GetAsync($"/api/accounts/{userId}"); //TODO: replace with correct endpoint
            return response.IsSuccessStatusCode;
        }
    }
}
