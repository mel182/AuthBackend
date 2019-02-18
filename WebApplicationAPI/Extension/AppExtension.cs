using WebApplicationAPI.Model;
using WebApplicationAPI.Utility;

namespace WebApplicationAPI.Extension
{
    public static class AppExtension
    {
        public static Post Refine(this Post rawInstance)
        {
            return new Post
            {
                Content = rawInstance.Content,
                Publish_date = TimeStamp.GetCurrent,
                Title = rawInstance.Title
            };
        }

        public static string ToResponseMessage(this string error_message)
        {   
            return "{"+string.Format(@"""message"" : ""{0}""", error_message)+"}";
        }
    }
}
