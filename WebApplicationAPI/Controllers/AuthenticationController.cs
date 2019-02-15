using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using WebApplicationAPI.DBHandler;
using WebApplicationAPI.Extension;
using WebApplicationAPI.Model;
using WebApplicationAPI.Models;
using WebApplicationAPI.security;

namespace WebApplicationAPI.Controllers
{
    [EnableCors(origins: "AllowSpecificOrigin", headers: "*", methods: "*")]
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private UserManager<AuthenticationUser> AuthenticatedUserManager;
        private readonly WebApplicationAPIContext Context;
        private const string SECURE_KEY = "4F08A4313838148CFE9B0B7A792093B1FD32F7A4B040C52B5092B4191AB16928";

        public AuthenticationController(UserManager<AuthenticationUser> authenticationUserManager)
        {
            this.AuthenticatedUserManager = authenticationUserManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login credential)
        {
            try
            {
                AuthenticationUser user = DbHandler.GetAuthenticatedUser(credential.Username, credential.Password);

                if (user != null)
                {
                        var requestClaim = new[]
                        {
                            new Claim(AuthorizationVerifier.CLAIM_USER_ID, user.Id),
                            new Claim(AuthorizationVerifier.CLAIM_USER_NAME, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURE_KEY));

                        var jwtSecureTokenRaw = new JwtSecurityToken(
                            issuer: "https://www.post.com",
                            audience: "Capgemini Academy",
                            expires: DateTime.UtcNow.AddMinutes(20),
                            claims: requestClaim,
                            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                            );
                   
                    return Ok(new
                    {
                        username = credential.Username,
                        email = user.Email,
                        token = new JwtSecurityTokenHandler().WriteToken(jwtSecureTokenRaw),
                        expiration = jwtSecureTokenRaw.ValidTo
                    });
                }
            }
            catch(InvalidOperationException) { }

            Response.StatusCode = 401;
            return Content("You are not authorized".ToErrorMessage());
        }

        [HttpPost]
        [Route("user/create")]
        public async Task<IActionResult> Create([FromBody] NewUser newUserCredential)
        {
            try
            {
                if (newUserCredential != null)
                {
                    if(DbHandler.CreateNewUser(newUserCredential))
                    {
                        JwtInstance jwtInstance = JwtCreator.Create(newUserCredential);
                        return Ok(new
                        {
                            username = newUserCredential.UserName,
                            email = newUserCredential.Email,
                            token = jwtInstance.Token,
                            expiration = jwtInstance.Validation
                        });
                    }
                }
            }
            catch (InvalidOperationException) { }

            Response.StatusCode = 401;
            return Content("You are not authorized".ToErrorMessage());
        }
    }
}