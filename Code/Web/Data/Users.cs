using System;
using System.Collections.Generic;

namespace JwtAuthenticationHelper.Data
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}
