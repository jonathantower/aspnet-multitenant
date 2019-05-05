using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.Web.Data.Interfaces
{
    public interface ITenant {
        int TenantId { get; set; }
    }
}
