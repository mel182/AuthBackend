using System;

namespace WebApplicationAPI.Utility
{
    public class TimeStamp
    {
        public static long GetCurrent
        {
            get
            {
                return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            } 
        }
    }
}
