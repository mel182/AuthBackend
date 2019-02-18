using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using WebApplicationAPI.Enum;

namespace WebApplicationAPI.Model
{
    public class AuthenticationUser : IdentityUser {
        //public long Registration_date { get; set; } = 0;
        //public long LastUpdate { get; set; } = 0;
        public RoleType Role { get; set; } = RoleType.USER;
    }
}
