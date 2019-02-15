using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAPI.Model
{
    public class JwtInstance
    {
        public string Token { get; private set; } = "";
        public DateTime Validation { get; private set; }

        public JwtInstance(string token, DateTime validation)
        {
            this.Token = token;
            this.Validation = validation;
        }
    }
}
