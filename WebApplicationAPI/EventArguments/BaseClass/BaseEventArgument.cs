using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAPI.EventArguments
{
    public abstract class BaseEventArgument
    {
        public bool Succeed { get; set; } = false;
        public string ErrorMessage { get; set; } = "";
    }
}
