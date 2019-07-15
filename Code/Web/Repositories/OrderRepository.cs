using JwtAuthenticationHelper.Data;
using Microsoft.AspNetCore.Http;
using MultiTenant.Web.Data;
using MultiTenant.Web.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.Web.Repositories
{
    public class OrderRepository : RepositoryBase<Order, TenantDbContext>, 
        IOrderRepository
    {
        public OrderRepository(TenantDbContext context, 
            IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor) { }
    }
}
