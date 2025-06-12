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

    }
    public class HelpPosts
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Telephone { get; set; }
        public string UserId { get; set; }
        public DateTime PostedAt { get; set; }
        public string UserName { get; set; }
    }
}