
using JwtAuthenticationHelper.Data;

namespace MultiTenant.Web.Helpers.Interfaces
{
    public interface ITenantHelper
    {
        Tenant GetTenant(int tenantId);
    }
}
