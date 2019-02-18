using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI.DBHandler;
using WebApplicationAPI.EventArguments;
using WebApplicationAPI.Extension;
using WebApplicationAPI.Model;
using WebApplicationAPI.Models;
using WebApplicationAPI.security;

namespace WebApplicationAPI.Services
{
    public class UserService
    {
        private static readonly Lazy<UserService> lazy = new Lazy<UserService>(() => new UserService());

        public Action<UserCreationEventArgs> userCreationActionResult;
        public Action<AdminCreationEventArgs> adminCreationActionResult;
        public Action<RootUserCreationEventArgs> rootUserCreationActionResult;
        private UserService() { }

        public static UserService Get
        {
            get
            {
                return lazy.Value;
            }
        }
        
        public UserCreationEventArgs InserNewUser(NewUser newUserCredential)
        {
            try
            {
                if (newUserCredential != null)
                {
                    if (DbHandler.CreateNewUser(newUserCredential, AuthorizationVerifier.USER))
                    {
                        JwtInstance jwtInstance = JwtCreator.Create(newUserCredential);

                        return new UserCreationEventArgs
                        {
                            Succeed = true,
                            UserName = newUserCredential.UserName,
                            Email = newUserCredential.Email,
                            Token = jwtInstance.Token,
                            Expiration = jwtInstance.Validation
                        };
                    }
                    else
                    {
                        return new UserCreationEventArgs
                        {
                            Succeed = false,
                            ErrorMessage = "Failed register user"
                        };
                    }
                }
                else
                {
                    return new UserCreationEventArgs
                    {
                        Succeed = false,
                        ErrorMessage = "Invalid user credential"
                    };
                }
            }
            catch (InvalidOperationException) {
                return new UserCreationEventArgs
                {
                    Succeed = false,
                    ErrorMessage = "Unable to register new user in the system"
                };
            }
        }

        public List<RegisteredUser> GetRegisteredUsers
        {
            get
            {
                return DbHandler.GetRegisteredUsers();
            }
        }


        public RegisteredUser GetRegisteredUser(string id)
        {
            return DbHandler.GetRegisteredUser(id);
        }
    }
}
