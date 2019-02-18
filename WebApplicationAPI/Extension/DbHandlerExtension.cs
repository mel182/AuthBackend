using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI.Model;
using WebApplicationAPI.Models;
using WebApplicationAPI.Utility;

namespace WebApplicationAPI.Extension
{
    public static class DbHandlerExtension
    {
        public static List<RegisteredUser> AddRoles(this List<RegisteredUser> registeredUsersList)
        {
            using (SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("select * from AspNetUsers", conn)
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
                                string normalizedEmail = reader["NormalizedEmail"].ToString();
                                string securityStamp = reader["SecurityStamp"].ToString();
                                string concurrencyStamp = reader["ConcurrencyStamp"].ToString();
                                var userFound = new RegisteredUser
                                {
                                    Id = id,
                                    UserName = userName,
                                    Email = email,
                                    SecurityStamp = securityStamp,
                                    ConcurrencyStamp = concurrencyStamp
                                };

                                registeredUsersList.Add(userFound);
                            }
                        }
                    }
                }
            }


            return null;
        }

        // ------------------------

        public static bool AddUser(this AuthenticationUser user)
        {
            bool result = false;
            SqlConnection conn = new SqlConnection(WebApplicationAPIContext.DBConnectionString);
            SqlCommand command;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = string.Format(
                @"INSERT INTO AspNetUsers(Id, UserName, Email, PasswordHash, 
                  SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, 
                  TwoFactorEnabled, LockoutEnabled, AccessFailedCount, EmailConfirmed, Registration_date,Last_update) 
                  VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', 0, 0, 0, 0, 0,{6},{7});",
                  Guid.NewGuid().ToString(), user.UserName, user.Email,
                  Base64Encoder.Encode(user.PasswordHash), Guid.NewGuid().ToString(),
                  Guid.NewGuid().ToString(), TimeStamp.GetCurrent, TimeStamp.GetCurrent);


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
            catch (IOException) { }
            catch (InvalidOperationException) { }

            conn.Close();

            return result;
        }

        private static object DateTimeOffset(DateTime utcNow)
        {
            throw new NotImplementedException();
        }



        // -------------------------
    }
}
