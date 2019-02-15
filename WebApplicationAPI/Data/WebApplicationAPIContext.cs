using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI.Model;

namespace WebApplicationAPI.Models
{
    public class WebApplicationAPIContext : IdentityDbContext<IdentityUser>
    {
        public static string DBConnectionString { get; set; } = "";

        public WebApplicationAPIContext (DbContextOptions<WebApplicationAPIContext> options)
            : base(options)
        {
        }

        public DbSet<WebApplicationAPI.Model.Post> Post { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
