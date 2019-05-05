using Microsoft.Extensions.Options;
using MultiTenant.Web.Helpers.Interfaces;
using MultiTenant.Web.Models;
using MultiTenant.Web.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DatabaseOptions _options;

        public UserHelper(IOptions<DatabaseOptions> options)
        {
            this._options = options.Value;
        }

        public User GetUser(string username)
        {
            using (var cn = new SqlConnection(_options.AuthConnectionString))
            {
                cn.Open();

                var sql = "SELECT UserId, Username, Password, TenantId FROM dbo.Users u WHERE u.Username = @Username";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Username", username ?? "");

                    using (var dr = cmd.ExecuteReader())
                    {
                        User user = null;

                        if (dr.Read())
                        {
                            user = new User
                            {
                                UserId = (int)dr["UserId"],
                                Username = (string)dr["Username"],
                                Password = (string)dr["Password"],
                                TenantId = (int)dr["TenantId"]
                            };
                        }

                        return user;
                    }

                }
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (var cn = new SqlConnection(_options.AuthConnectionString))
            {
                cn.Open();

                var sql = "SELECT COUNT(*) FROM dbo.Users u WHERE u.Username = @Username AND u.Password = @Password";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Username", username ?? "");
                    cmd.Parameters.AddWithValue("@Password", password ?? "");

                    var count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}
