using System.Collections.Generic;
using System.Security.Claims;
using WebApplicationAPI.DBHandler;

namespace WebApplicationAPI.security
{
    public class AuthorizationVerifier
    {
        public const string ADMIN = "ADMIN";
        public const string USER = "USER";
        public const string CLAIM_USER_NAME = "user_name";
        public const string CLAIM_USER_ID = "user_id";

        public static bool IsAuthorized(string target_role, IEnumerable<Claim> jwt_claims)
        {
            foreach (Claim jwt_claim in jwt_claims)
            {
                if (jwt_claim.Type.Equals(CLAIM_USER_ID))
                {
                    var user_id = jwt_claim.Value;
                    List<string> roles = DbHandler.GetUserRole(user_id);

                    foreach(string roleFound in roles)
                    {
                        if (roleFound.Equals(target_role))
                        {
                            return true;
                        }   
                    }
                }
            }

            return false;
        }
        
    }
}
