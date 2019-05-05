using JwtAuthenticationHelper.Data;
using Microsoft.Extensions.Options;
using MultiTenant.Web.Helpers.Interfaces;
using MultiTenant.Web.Options;
using System.Data.SqlClient;

namespace MultiTenant.Web.Helpers
{
    public class TenantHelper : ITenantHelper
    {
        private readonly DatabaseOptions _options;

        public TenantHelper(IOptions<DatabaseOptions> options)
        {
            this._options = options.Value;
        }

        public Tenant GetTenant(int tenantId)
        {
            using (var cn = new SqlConnection(_options.AuthConnectionString))
            {
                cn.Open();

                var sql = "SELECT TenantId, TenantName, DatabaseServer, [Database] FROM dbo.Tenants t WHERE t.TenantId = @TenantId";
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@TenantId", tenantId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        Tenant tenant = null;

                        if (dr.Read())
                        {
                            tenant = new Tenant
                            {
                                TenantId = (int)dr["TenantId"],
                                TenantName = (string)dr["TenantName"],
                                DatabaseServer = (string)dr["DatabaseServer"],
                                Database = (string)dr["Database"]
                            };
                        }

                        return tenant;
                    }

                }
            }
        }
    }
}
