using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using WebApplicationAPI.Extension;
using WebApplicationAPI.Model;
using WebApplicationAPI.Models;
using WebApplicationAPI.security;
using WebApplicationAPI.Utility;

namespace WebApplicationAPI.DBHandler
{
    public class DbHandler
    {
        public static AuthenticationUser GetAuthenticatedUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("select * from AspNetUsers", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string user_name = reader["UserName"].ToString();
                                string password_hash = reader["PasswordHash"].ToString();

                                var decodedPassword = Base64Encoder.Decode(password_hash);

                                if (user_name != null && user_name.Equals(username) && password.Equals(decodedPassword))
                                {

                                    string id = reader["Id"].ToString();
                                    string email = reader["Email"].ToString();
                                    string securityStamp = reader["SecurityStamp"].ToString();

                                    AuthenticationUser authenticationUser = new AuthenticationUser
                                    {
                                        Id = id,
                                        UserName = user_name,
                                        Email = email,
                                        SecurityStamp = securityStamp,
                                        PasswordHash = password_hash
                                    };

                                    return authenticationUser;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

       
        public static List<string> GetUserRole(string userID)
        {
            List<string> roles_found = new List<string>();

            using (SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(string.Format("select AspNetRoles.Name from AspNetUserRoles inner join AspNetRoles on AspNetUserRoles.RoleId=AspNetRoles.Id AND AspNetUserRoles.UserId = '{0}';", userID), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string role = reader["Name"].ToString();

                                if (role != null)
                                {
                                    roles_found.Add(role);
                                }
                            }
                        }
                    }
                }
            }

            return roles_found;
        }

        public static bool CreateNewUser(NewUser newUser, string user_type)
        {
            if(InsertNewUser(newUser))
            {
                var userID = GetUserID(newUser.Email);
                var roleID = GetUserRoleID(user_type);

                if(!userID.Equals("") && !roleID.Equals(""))
                {
                    return InsertUserRole(userID, roleID); 
                } 
            }
            
            return false;
        }

        //RegisteredUser
        //-----
        public static List<RegisteredUser> GetRegisteredUsers()
        {

            // DataContext takes a connection string 
            //DataContext db = new DataContext(WebApplicationAPIContext.DBConnectionString);
            //// Get a typed table to run queries
            //Table<RegisteredUser> registeredUsers = db.GetTable<RegisteredUser>();
            //var query = (from p in registeredUsers.Cast<RegisteredUser>() select p);
            //foreach (var cust in query)
            //    Debug.WriteLine("id = {0}, Username = {1}", cust.Id, cust.UserName);

            //return query;
            // Query for customers from London
            //var q =
            //   from c in registeredUsers
            //   select c;


            //IEnumerable<RegisteredUser> registeredUsers = from users in Context.RegisteredUsers select users;
            //                    where p.PublishedAt.Year > 2018
            //orderby p.Id ascending
            //select users;

            //IEnumerable<Post> posts = from
            //                  p in _context.Post
            //                          where p.PublishedAt.Year > 2018
            //                          orderby p.Id ascending
            //                          select p;


            //return Context.RegisteredUsers;

            List<RegisteredUser> registeredUsersList = new List<RegisteredUser>();

            using (SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("select users.Id,users.UserName,users.Email,users.SecurityStamp,users.ConcurrencyStamp,roles.Name as Role from AspNetUserRoles as userRoles inner join AspNetRoles as roles on (userRoles.RoleId = roles.Id) inner join AspNetUsers as users on (users.Id = userRoles.UserId);", conn)
                    )
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string id = reader["Id"].ToString();
                                string userName = reader["UserName"].ToString();
                                string email = reader["Email"].ToString();
                                string securityStamp = reader["SecurityStamp"].ToString();
                                string concurrencyStamp = reader["ConcurrencyStamp"].ToString();
                                string role = reader["Role"].ToString();

                                var userFound = new RegisteredUser
                                {
                                    Id = id,
                                    UserName = userName,
                                    Email = email,
                                    SecurityStamp = securityStamp,
                                    ConcurrencyStamp = concurrencyStamp,
                                    Role = role,
                                    RegistrationDate = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                                };
                                
                                registeredUsersList.Add(userFound);
                            }
                        }
                    }
                }
            }
            
            return registeredUsersList;
        }



        private static List<RegisteredUser> AddRole(List<RegisteredUser> registeredUsersWithoutRole)
        {
            return registeredUsersWithoutRole;
        }


        // ---------------

        private static string GetUserRoleID(string role)
        {
            string ID = "";

            using (SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(string.Format("select AspNetRoles.Id from AspNetRoles where Name = '{0}';", role), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string roleID = reader["Id"].ToString();

                                if (roleID != null)
                                {
                                    ID = roleID;
                                }
                            }
                        }
                    }
                }
            }

            return ID;
        }


        private static string GetUserID(string email)
        {
            string ID = "";

            using (SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(string.Format("select * from AspNetUsers where Email='{0}';", email), conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                string emailFound = reader["Email"].ToString();
                                if (email.Equals(emailFound))
                                {
                                    string roleID = reader["Id"].ToString();

                                    if (roleID != null)
                                    {
                                        ID = roleID;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ID;
        }


        private static bool InsertUserRole(string userID, string roleID)
        {
            bool result = false;
            SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString);
            SqlCommand command;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = string.Format(
                @"INSERT INTO AspNetUserRoles (UserId,RoleId) 
                          VALUES ('{0}','{1}');",
                userID, roleID);
            
            try
            {
                conn.Open();
                command = new SqlCommand(sql, conn);

                adapter.InsertCommand = new SqlCommand(sql, conn);
                if (adapter.InsertCommand.ExecuteNonQuery() == 1) // 1 means it successfully stored data in database
                {
                    result = true;
                }
                command.Dispose();

            }
            catch (InvalidCastException) { }
            catch (SqlException){ }
            catch (IOException) { }
            catch (InvalidOperationException) { }
            
            conn.Close();

            return result;
            
        }

        private static bool InsertNewUser(NewUser newUser)
        {
            bool result = false;
            SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString);
            SqlCommand command;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = string.Format(
                @"INSERT INTO AspNetUsers(Id, UserName, Email, PasswordHash, 
                  SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, 
                  TwoFactorEnabled, LockoutEnabled, AccessFailedCount, EmailConfirmed, Registration_date, Last_update) 
                  VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', 0, 0, 0, 0, 0,{6},{7});",
                  Guid.NewGuid().ToString(), newUser.UserName, newUser.Email,
                  Base64Encoder.Encode(newUser.Password), Guid.NewGuid().ToString(),
                  Guid.NewGuid().ToString(), 
                  DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                  DateTimeOffset.Now.ToUnixTimeMilliseconds());
            
            try
            {
                conn.Open();
                command = new SqlCommand(sql, conn);

                adapter.InsertCommand = new SqlCommand(sql, conn);
                if (adapter.InsertCommand.ExecuteNonQuery() == 1) // 1 means it successfully stored data in database
                {
                    result = true;
                }
                command.Dispose();

            }
            catch (InvalidCastException) { }
            catch (SqlException) { }
            catch (IOException){ }
            catch (InvalidOperationException){ }
            
            conn.Close();

            return result;
        }

        public static RegisteredUser GetRegisteredUser(string id)
        {
            RegisteredUser result = null;

            if (id != null)
            {
                if (!id.Verify().Equals("") && !id.Equals(""))
                {
                    // --------------------

                    //List<RegisteredUser> registeredUsersList = new List<RegisteredUser>();

                    using (SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand(string.Format("select users.Id,users.UserName,users.Email,users.SecurityStamp,users.ConcurrencyStamp,users.Registration_date,users.Last_update,roles.Name as Role from AspNetUserRoles as userRoles inner join AspNetRoles as roles on (userRoles.RoleId = roles.Id) inner join AspNetUsers as users on (users.Id = userRoles.UserId) where users.Id = '{0}';", id), conn))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader != null)
                                {
                                    while (reader.Read())
                                    {
                                        string _id = reader["Id"].ToString();
                                        string userName = reader["UserName"].ToString();
                                        string email = reader["Email"].ToString();
                                        string securityStamp = reader["SecurityStamp"].ToString();
                                        string concurrencyStamp = reader["ConcurrencyStamp"].ToString();
                                        string role = reader["Role"].ToString();
                                        long registration = (long) reader["Registration_date"];
                                        long lastUpdate = (long) reader["Last_update"];
                                      
                                        result = new RegisteredUser
                                        {
                                            Id = id,
                                            UserName = userName,
                                            Email = email,
                                            SecurityStamp = securityStamp,
                                            ConcurrencyStamp = concurrencyStamp,
                                            Role = role,
                                            RegistrationDate = registration,
                                            LastUpdated = lastUpdate
                                        };
                                    }
                                }
                            }
                        }
                    }
                    // -----------------------
                }
            }

            return result;
        }
    }
}
