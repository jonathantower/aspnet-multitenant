using JwtAuthenticationHelper.Data;
using MultiTenant.Web.Data;

namespace MultiTenant.Web.Repositories.Interfaces
{
    public interface IOrderRepository: IRepositoryBase<Order, TenantDbContext> { }
}