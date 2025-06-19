using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace comApp.communities
{
    public static class ApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "http://mc.dominikmeister.com/api";

        private static void AddAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrEmpty(App.SessionToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {App.SessionToken}");
            }
        }

        public static async Task<ApiResponse> GetRequest(string url)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(BaseUrl + url);
                var content = await response.Content.ReadAsStringAsync();

                return new ApiResponse
                {
                    IsSuccessStatusCode = response.IsSuccessStatusCode,
                    Content = content,
                    ErrorMessage = !response.IsSuccessStatusCode ? content : null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccessStatusCode = false, ErrorMessage = ex.Message };
            }
        }

        public static async Task<ApiResponse> PutRequest(string url, object data)
        {
            try
            {
                AddAuthorizationHeader();

                HttpContent? content = null;
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.PutAsync(BaseUrl + url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return new ApiResponse
                {
                    IsSuccessStatusCode = response.IsSuccessStatusCode,
                    Content = responseContent,
                    ErrorMessage = !response.IsSuccessStatusCode ? responseContent : null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccessStatusCode = false, ErrorMessage = ex.Message };
            }
        }

        public static async Task<string> GetMembershipStatus(int communityId, string userId)
        {
            var response = await GetRequest($"/Community/MembershipRequests/{communityId}");

            if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(response.Content))
                return "Unknown";

            try
            {
                var list = JsonSerializer.Deserialize<List<MembershipRequest>>(response.Content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var match = list?.FirstOrDefault(req => req.userId == userId);

                if (match == null)
                    return "Unrequested";

                return match.status switch
                {
                    0 => "Pending",
                    1 => "Accepted",
                    2 => "Declined",
                    _ => "Unknown"
                };
            }
            catch
            {
                return "Error";
            }
        }

        private class MembershipRequest
        {
            public string userId { get; set; }
            public int communityId { get; set; }
            public int status { get; set; }
        }


    }
}
