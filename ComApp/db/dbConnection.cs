using MySqlConnector;
using System.Text;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace comApp.db
{
    public class dbConnection
    {
        private MySqlConnection _connection;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://townsquareapi.onrender.com/api";

        public dbConnection()
        {
            string connectionString = "server=10.0.2.2;port=3306;database=comapp;user=root;password=sml12345";

            _connection = new MySqlConnection(connectionString);

            _httpClient = new HttpClient();
        }

        // Helper method for GET requestss
        public async Task<string> GetRequest(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + endpoint);
            return await HandleResponse(response);
        }

        // Helper method for POST requests
        public async Task<string> PostRequest(string endpoint, object requestBody)
        {
            string jsonContent = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(_baseUrl + endpoint, content);
            return await HandleResponse(response);
        }

        // Helper method for PUT requests
        public async Task<string> PutRequest(string endpoint, object requestBody)
        {
            string jsonContent = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(_baseUrl + endpoint, content);
            return await HandleResponse(response);
        }

        // Helper method for Delete requests
        public async Task<string> DeleteRequest(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_baseUrl + endpoint);
            return await HandleResponse(response);
        }

        // handle the response and return it as a string
        public async Task<string> HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "Error: " + response.StatusCode;
            }
        }

        // Community
        public async Task<string> GetAllCommunities()
        {
            return await GetRequest("");
        }

        public async Task<string> RequestMembership(int userId, int communityId)
        {
            var requestBody = new { userId, communityId };
            return await PostRequest("/Community/RequestMembership", requestBody);
        }

        // Help Posts
        public async Task<string> GetHelpPosts()
        {
            return await GetRequest("/HelpPost/GetHelpPosts");
        }

        public async Task<string> AddHelpPost(int userId, string title, string description, double price, string telephone, DateTime helpPostTime)
        {
            var requestBody = new
            {
                title,
                description,
                price,
                telephone,
                helpposttime = helpPostTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                user_id = userId
            };
            return await PostRequest("/HelpPost/AddHelpPost", requestBody);
        }

        // Auth
        public async Task<string> Login(string username, string password)
        {
            var requestBody = new { username, password };
            string response = await PostRequest("/Login/Login", requestBody);

            dynamic jsonResponse = JsonConvert.DeserializeObject(response);
            if (jsonResponse != null && jsonResponse.token != null)
            {
                // Assuming the response contains a token (e.g., JWT)
                string token = jsonResponse.token;
                App.UserId = ExtractUserIdFromToken(token); // You'll need to implement this function based on your token structure
            }

            return response;
        }

        // Function to extract userId from a JWT or token
        private int ExtractUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "userId");

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }


        // Pins
        public async Task<string> GetPins()
        {
            return await GetRequest("/Pins/GetPins");
        }

        public async Task<string> InsertPin(int userId, string title, string description, DateTime postTime, double x_cord, double y_cord, int communityId, int pinType)
        {
            var requestBody = new
            {
                user_id = userId,
                title,
                description,
                posttime = postTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                x_cord,
                y_cord,
                community_id = communityId,
                pintype = pinType
            };
            return await PostRequest("/Pin/InsertPin", requestBody);
        }

        // Posts
        public async Task<string> GetCommunityPosts(int isNews)
        {
            return await GetRequest($"/Post/GetPosts/{isNews}");
        }

        public async Task<string> CreatePost(int userId, string content, int isNews, int communityId)
        {
            var requestBody = new
            {
                content,
                user_id = userId,
                isnews = isNews,
                community_id = communityId
            };
            return await PostRequest("/Post/CreatePost", requestBody);
        }

        // User
        public async Task<string> RegisterUser(string name, string email, string password, string bio)
        {
            var requestBody = new
            {
                name,
                email,
                password,
                bio
            };
            return await PostRequest("/User/Register", requestBody);
        }

        public async Task<string> UpdateUserBio(int userId, string newUsername, string newBio)
        {
            var requestBody = new { newUsername, newBio };
            return await PutRequest($"/User/UpdateBio/{userId}", requestBody);
        }

        public async Task<string> GetUserById(int userId)
        {
            return await GetRequest($"/User/{userId}");
        }

        public async Task<string> DeleteUser(int userId)
        {
            return await DeleteRequest($"/User/UpdateBio/{userId}");
        }

    }
}