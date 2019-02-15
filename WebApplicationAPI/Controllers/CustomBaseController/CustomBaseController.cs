using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using WebApplicationAPI.security;

namespace WebApplicationAPI.Controllers.CustomBaseController
{
    public abstract class BaseSecurityController : ControllerBase
    {
        private IEnumerable<Claim> GetJwtClaims()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity.Claims;
        }
        
        protected bool IsAuthorized(string targetRole)
        {
            return AuthorizationVerifier.IsAuthorized(targetRole, GetJwtClaims()); ;
        }

        protected bool IsAdmin()
        {
            return AuthorizationVerifier.IsAuthorized(AuthorizationVerifier.ADMIN, GetJwtClaims()); ;
        }


    }


}
