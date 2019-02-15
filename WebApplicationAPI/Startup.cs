using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI.Models;
using System.Web.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using WebApplicationAPI.Model;
using Microsoft.AspNetCore.Identity;
using WebApplicationAPI.oAuth2;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApplicationAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            WebApplicationAPIContext.DBConnectionString = Configuration.GetConnectionString("WebApplicationAPIContext");
      
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<WebApplicationAPIContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("WebApplicationAPIContext")));

            services.AddIdentity<AuthenticationUser, IdentityRole>()
                .AddEntityFrameworkStores<WebApplicationAPIContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "Capgemini Academy",
                    ValidIssuer = "https://www.post.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("4F08A4313838148CFE9B0B7A792093B1FD32F7A4B040C52B5092B4191AB16928"))

                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(builder =>
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            //HttpConfiguration
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            SeedDatabase.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        protected static string GetXmlCommentsPath()
        {
            return System.String.Format(@"{0}\WebApplicationAPI\WebApplicationAPI.xml",
                System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }


    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();

            httpConfiguration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional}
            );

            var jsonFormatter = httpConfiguration.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }

}
