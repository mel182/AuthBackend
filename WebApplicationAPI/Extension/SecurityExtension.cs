using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAPI.Extension
{
    public static class SecurityExtension
    {
        public static string Verify(this string rawValue)
        {
            if (rawValue.Contains("OR 1=1") || rawValue.Contains("1=1"))
                return "";
            
            return rawValue;
        }

    }
}
