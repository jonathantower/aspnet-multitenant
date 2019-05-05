using MultiTenant.Web.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace MultiTenant.Web.Data
{
    public partial class Order: ITenant
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TenantId { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
