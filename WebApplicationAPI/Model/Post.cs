namespace WebApplicationAPI.Model
{
    public class Post
    {
        public long Id { get; set; } = 0;
        public string Title { get; set; } = "";
        public long Publish_date { get; set; } = 0;
        public string Content { get; set; } = "";
        public string Creator { get; set; } = "";
        public long Last_update { get; set; } = 0;
    }
}
