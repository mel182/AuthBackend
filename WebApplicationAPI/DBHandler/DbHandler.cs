using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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

        public static bool CreateNewUser(NewUser newUser)
        {
            if(InsertNewUser(newUser))
            {
                var userID = GetUserID(newUser.Email);
                var roleID = GetUserRoleID(AuthorizationVerifier.USER);

                if(!userID.Equals("") && !roleID.Equals(""))
                {
                    return InsertUserRole(userID, roleID);
                    
                } 
            }
            
            return false;
        }


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
                  TwoFactorEnabled, LockoutEnabled, AccessFailedCount, EmailConfirmed) 
                  VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', 0, 0, 0, 0, 0);",
                  Guid.NewGuid().ToString(), newUser.UserName, newUser.Email,
                  Base64Encoder.Encode(newUser.Password), Guid.NewGuid().ToString(),
                  Guid.NewGuid().ToString());


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
    }
}
