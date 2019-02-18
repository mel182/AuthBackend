using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAPI.EventArguments
{
    public class UserCreationEventArgs : BaseEventArgument
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Token { get; set; } = "";
        public DateTime Expiration { get; set; }
    }
}
