using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplicationAPI.DBHandler;
using WebApplicationAPI.Model;

namespace WebApplicationAPI.security
{
    public class JwtCreator
    {
        private const string SECURE_KEY = "4F08A4313838148CFE9B0B7A792093B1FD32F7A4B040C52B5092B4191AB16928";

        public static JwtInstance Create(NewUser newUser)
        {
            try
            {
                AuthenticationUser user = DbHandler.GetAuthenticatedUser(newUser.UserName, newUser.Password);

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
                    
                    var token = new JwtSecurityTokenHandler().WriteToken(jwtSecureTokenRaw);

                    return new JwtInstance(token, jwtSecureTokenRaw.ValidTo);
                }
            }
            catch (InvalidOperationException) { }

            return null;
        }
    }
}
