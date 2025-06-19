using Newtonsoft.Json;

namespace comApp.posts
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime PostTime { get; set; }
        [JsonProperty("isNews")]
        public int IsNews { get; set; }

        [JsonProperty("likeCount")]
        public int LikeCount { get; set; }

        [JsonProperty("isLikedByCurrentUser")]
        public bool IsLikedByCurrentUser { get; set; }
        public string LikeIcon => IsLikedByCurrentUser ? "\uf004" : "\uf08a";
        public Color LikeBackgroundColor => IsLikedByCurrentUser ? Color.FromArgb("#E0245E") : Color.FromArgb("#bbb");
    }
    public class HelpPosts
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Price { get; set; }
        public string Telephone { get; set; }
        public string UserId { get; set; }
        public DateTime PostedAt { get; set; }
        public string UserName { get; set; }
    }
}