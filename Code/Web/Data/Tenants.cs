using System;
using System.Collections.Generic;

namespace JwtAuthenticationHelper.Data
{
    public partial class Tenant
    {
        public Tenant()
        {
            Users = new HashSet<User>();
        }

        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string DatabaseServer { get; set; }
        public string Database { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
