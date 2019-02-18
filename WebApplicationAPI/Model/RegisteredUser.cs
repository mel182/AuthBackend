using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAPI.Model
{
    public class RegisteredUser
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string NormalizedUser { get; set; } = "";
        public string Email { get; set; } = "";
        public string SecurityStamp { get; set; } = "";
        public string ConcurrencyStamp { get; set; } = "";
        public string Role { get; set; } = "";
        public long RegistrationDate { get; set; } = 0;
        public long LastUpdated { get; set; } = 0;
    }
}
