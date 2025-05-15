using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace comApp.db
{
    public class dbConnection
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://mc.dominikmeister.com/api";

        public dbConnection()
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true
            };
            _httpClient = new HttpClient(handler);
        }

        private async Task<string> GetRequest(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + endpoint);
            return await HandleResponse(response);
        }

        private async Task<string> PostRequest(string endpoint, object requestBody)
        {
            string jsonContent = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_baseUrl + endpoint, content);
            return await HandleResponse(response);
        }

        private async Task<string> PutRequest(string endpoint, object requestBody)
        {
            string jsonContent = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(_baseUrl + endpoint, content);
            return await HandleResponse(response);
        }

        private async Task<string> DeleteRequest(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_baseUrl + endpoint);
            return await HandleResponse(response);
        }

        private static async Task<string> HandleResponse(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
        }

        // -------------------------------
        // COMMUNITY
        // -------------------------------

        public Task<string> GetAllCommunities() => GetRequest("/Community/GetAll");

        public Task<string> GetCommunityById(int id) => GetRequest($"/Community/GetById/{id}");

        public Task<string> GetCommunityByName(string name) => GetRequest($"/Community/GetByName?name={name}");

        public Task<string> CreateCommunity(string name, string description, string location, bool isLicensed)
        {
            var body = new { name, description, location, isLicensed };
            return PostRequest("/Community/Create", body);
        }

        public Task<string> UpdateCommunity(int communityId, string name, string description, string location, bool isLicensed)
        {
            var body = new { name, description, location, isLicensed };
            return PutRequest($"/Community/Update/{communityId}", body);
        }

        public Task<string> DeleteCommunity(int communityId) => DeleteRequest($"/Community/Delete/{communityId}");

        public Task<string> RequestMembership(int userId, int communityId)
        {
            return PutRequest($"/Community/RequestMembership?userId={userId}&communityId={communityId}", null);
        }

        // -------------------------------
        // HELP POST
        // -------------------------------

        public Task<string> GetHelpPosts() => GetRequest("/HelpPost/GetHelpPosts");

        public Task<string> AddHelpPost(string title, string description, double price, string telephone, DateTime postedAt, int userId)
        {
            var body = new { title, description, price, telephone, postedAt, userId };
            return PostRequest("/HelpPost/AddHelpPost", body);
        }

        // -------------------------------
        // LOGIN
        // -------------------------------

        public Task<string> Login(string username, string password)
        {
            var body = new { username, password };
            return PostRequest("/Login/Login", body);
        }

        // -------------------------------
        // PIN
        // -------------------------------

        public Task<string> GetAllPins(int communityId) => GetRequest($"/Pin/GetAll?communityId={communityId}");

        public Task<string> GetPinById(int communityId, int id) =>
            GetRequest($"/Pin/GetById?communityId={communityId}&id={id}");

        public Task<string> CreatePin(string title, string description, double xCord, double yCord, int communityId, int pintype)
        {
            var body = new { title, description, xCord, yCord, communityId, pintype };
            return PostRequest("/Pin", body);
        }

        public Task<string> UpdatePin(int communityId, int pinId, string title, string description, int xCord, int yCord, int pintype)
        {
            var body = new { title, description, xCord, yCord, communityId, pintype };
            return PutRequest($"/Pin?communityId={communityId}&pinId={pinId}", body);
        }

        public Task<string> DeletePin(int communityId, int pinId) =>
            DeleteRequest($"/Pin?communityId={communityId}&pinId={pinId}");

        // -------------------------------
        // POST
        // -------------------------------

        public Task<string> GetAllPosts(int communityId) =>
            GetRequest($"/Post/GetAll?communityId={communityId}");

        public Task<string> GetPostById(int communityId, int id) =>
            GetRequest($"/Post/GetById?communityId={communityId}&id={id}");

        public Task<string> CreatePost(string content, int isNews, int communityId)
        {
            var body = new { content, isNews, communityId };
            return PostRequest("/Post", body);
        }

        public Task<string> UpdatePost(int communityId, int postId, string content, int isNews)
        {
            var body = new { content, isNews, communityId };
            return PutRequest($"/Post?communityId={communityId}&postId={postId}", body);
        }

        public Task<string> DeletePost(int communityId, int postId) =>
            DeleteRequest($"/Post?communityId={communityId}&postId={postId}");

        // -------------------------------
        // USER
        // -------------------------------

        public Task<string> RegisterUser(string name, string email, string password, string description)
        {
            var body = new { name, email, password, description };
            return PostRequest("/User/Register", body);
        }

        public Task<string> LoginUser(string email, string password)
        {
            var body = new { email, password };
            return PostRequest("/User/Login", body);
        }

        public Task<string> GetUserById(int userId) => GetRequest($"/User/GetById/{userId}");

        // public Task<string> GetUserBySessionToken(string sessionToken) => GetRequest($"/User/GetByToken/{sessionToken}");

        // public Task<string> ValidateSessionToken(string sessionToken) => GetRequest($"/User/Validate/{sessionToken}");
        public Task<string> UpdateUser(int userId, string email, string password, string name, string description)
        {
            var body = new { email, password, name, description };
            return PutRequest($"/User/Update/{userId}", body);
        }

        public Task<string> DeleteUser(int userId) => DeleteRequest($"/User/DeleteUser/{userId}");
    }
}
