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
        private void AddAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrEmpty(App.SessionToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {App.SessionToken}");
            }
        }

        private async Task<ApiResponse> GetRequest(string endpoint)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + endpoint);
            return await HandleResponse(response);
        }

        private async Task<ApiResponse> PostRequest(string endpoint, object requestBody)
        {
            AddAuthorizationHeader();
            string jsonContent = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_baseUrl + endpoint, content);
            return await HandleResponse(response);
        }

        private async Task<ApiResponse> PutRequest(string endpoint, object requestBody)
        {
            AddAuthorizationHeader();
            string jsonContent = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(_baseUrl + endpoint, content);
            return await HandleResponse(response);
        }

        private async Task<ApiResponse> DeleteRequest(string endpoint)
        {
            AddAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.DeleteAsync(_baseUrl + endpoint);
            return await HandleResponse(response);
        }

        private static async Task<ApiResponse> HandleResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse
                {
                    IsSuccess = true,
                    Content = content,
                    StatusCode = (int)response.StatusCode
                };
            }
            else
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Content = null,
                    ErrorMessage = content,
                    StatusCode = (int)response.StatusCode
                };
            }
        }

        // -------------------------------
        // COMMUNITY
        // -------------------------------

        public Task<ApiResponse> GetAllCommunities() => GetRequest("/Community/GetAll");

        public Task<ApiResponse> GetCommunityById(int id) => GetRequest($"/Community/GetById/{id}");

        public Task<ApiResponse> GetCommunityByName(string name) => GetRequest($"/Community/GetByName?name={name}");

        public Task<ApiResponse> CreateCommunity(string name, string description, string location, bool isLicensed)
        {
            var body = new { name, description, location, isLicensed };
            return PostRequest("/Community/Create", body);
        }

        public Task<ApiResponse> UpdateCommunity(int communityId, string name, string description, string location, bool isLicensed)
        {
            var body = new { name, description, location, isLicensed };
            return PutRequest($"/Community/Update/{communityId}", body);
        }

        public Task<ApiResponse> DeleteCommunity(int communityId) => DeleteRequest($"/Community/Delete/{communityId}");

        public Task<ApiResponse> RequestMembership(int userId, int communityId)
        {
            return PutRequest($"/Community/RequestMembership?userId={userId}&communityId={communityId}", null);
        }

        // -------------------------------
        // HELP POST
        // -------------------------------

        public Task<ApiResponse> GetHelpPosts() => GetRequest("/HelpPost/GetHelpPosts");

        public Task<ApiResponse> AddHelpPost(string title, string description, double price, string telephone, DateTime postedAt, string userId)
        {
            var body = new
            {
                title,
                description,
                price = (int)price,
                telephone,
                postedAt = postedAt.ToString("o"),
                userId
            };
            return PostRequest("/HelpPost/AddHelpPost", body);
        }


        // -------------------------------
        // LOGIN
        // -------------------------------

        public Task<ApiResponse> Login(string username, string password)
        {
            var body = new { username, password };
            return PostRequest("/Login/Login", body);
        }

        // -------------------------------
        // PIN
        // -------------------------------

        public Task<ApiResponse> GetAllPins(int communityId) => GetRequest($"/Pin/GetAll?communityId={communityId}");

        public Task<ApiResponse> GetPinById(int communityId, int id) =>
            GetRequest($"/Pin/GetById?communityId={communityId}&id={id}");

        public Task<ApiResponse> CreatePin(string title, string description, double xCord, double yCord, int communityId, int pintype)
        {
            var body = new { title, description, xCord, yCord, communityId, pintype };
            return PostRequest("/Pin", body);
        }

        public Task<ApiResponse> UpdatePin(int communityId, int pinId, string title, string description, int xCord, int yCord, int pintype)
        {
            var body = new { title, description, xCord, yCord, communityId, pintype };
            return PutRequest($"/Pin?communityId={communityId}&pinId={pinId}", body);
        }

        public Task<ApiResponse> DeletePin(int communityId, int pinId) =>
            DeleteRequest($"/Pin?communityId={communityId}&pinId={pinId}");

        // -------------------------------
        // POST
        // -------------------------------

        public Task<ApiResponse> GetAllPosts(int communityId) =>
            GetRequest($"/Post/GetAll?communityId={communityId}");

        public Task<ApiResponse> GetPostById(int communityId, int id) =>
            GetRequest($"/Post/GetById?communityId={communityId}&id={id}");

        public Task<ApiResponse> CreatePost(string content, int isNews, int communityId)
        {
            var body = new { content, isNews, communityId };
            return PostRequest("/Post", body);
        }

        public Task<ApiResponse> UpdatePost(int communityId, int postId, string content, int isNews)
        {
            var body = new { content, isNews, communityId };
            return PutRequest($"/Post?communityId={communityId}&postId={postId}", body);
        }

        public Task<ApiResponse> DeletePost(int communityId, int postId) =>
            DeleteRequest($"/Post?communityId={communityId}&postId={postId}");

        // -------------------------------
        // USER
        // -------------------------------

        public Task<ApiResponse> RegisterUser(string name, string email, string password, string description)
        {
            var body = new { name, email, password, description };
            return PostRequest("/Auth/Register", body);
        }

        public Task<ApiResponse> LoginUser(string email, string password)
        {
            var body = new { email, password };
            return PostRequest("/Auth/Login", body);
        }

        public async Task<ApiResponse> GetUserById(string userId)
        {
            var userResponse = await GetRequest($"/User/{userId}");
            if (userResponse == null || !userResponse.IsSuccess)
                return userResponse;

            string imageUrl = null;

            var imageResponse = await GetRequest($"https://image-api.com/api/images/{userId}"); 
            if (imageResponse != null && imageResponse.IsSuccess)
            {
                try
                {
                    dynamic imageData = JsonConvert.DeserializeObject(imageResponse.Content);
                    imageUrl = imageData?.image; 
                }
                catch { }
            }

            try
            {
                dynamic userData = JsonConvert.DeserializeObject(userResponse.Content);
                userData.profileImage = imageUrl ?? "";
                string mergedJson = JsonConvert.SerializeObject(userData);
                return new ApiResponse(true, mergedJson);
            }
            catch
            {
                return new ApiResponse(false, "Failed to merge user and image data.");
            }
        }

        public async Task<ApiResponse> UploadUserImage(string userId, FileResult imageFile)
        {
            if (imageFile == null)
                return new ApiResponse(false, "No image provided.");

            using var stream = await imageFile.OpenReadAsync();
            using var content = new MultipartFormDataContent();

            var imageContent = new StreamContent(stream);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            content.Add(imageContent, "image", imageFile.FileName);
            content.Add(new StringContent(userId), "userId");

            using var client = new HttpClient { BaseAddress = new Uri("https://image-api.com") }; // Replace this
            var response = await client.PostAsync("/api/images/upload", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? new ApiResponse(true, responseBody)
                : new ApiResponse(false, responseBody, (int)response.StatusCode, response.ReasonPhrase);
        }

        // public Task<string> GetUserBySessionToken(string sessionToken) => GetRequest($"/User/GetByToken/{sessionToken}");

        // public Task<string> ValidateSessionToken(string sessionToken) => GetRequest($"/User/Validate/{sessionToken}");
        public Task<ApiResponse> UpdateUser(string userId, string email, string firstName, string lastName, string description)
        {
            var body = new { email, firstName, lastName, description };
            return PutRequest($"/User/{userId}", body);
        }

        public Task<ApiResponse> DeleteUser(int userId) => DeleteRequest($"/User/{userId}");
    }

}

