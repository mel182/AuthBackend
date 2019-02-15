using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WebApplicationAPI.Model;
using WebApplicationAPI.Models;
using WebApplicationAPI.Utility;

namespace WebApplicationAPI.oAuth2
{
    public class SeedDatabase
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<WebApplicationAPIContext>();
           
            //Admin user
            var userManager = serviceProvider.GetRequiredService<UserManager<AuthenticationUser>>();

            //Make sure that the database is created
            context.Database.EnsureCreated();

            //Check if database contains admin users
            if(!context.Users.Any())
            {
                AuthenticationUser user = new AuthenticationUser()
                {
                    Email = "admin@postapplication.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "admin",
                    PasswordHash = Base64Encoder.Encode("admin")
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
