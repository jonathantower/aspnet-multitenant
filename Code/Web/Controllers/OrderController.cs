using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenant.Web.Data;
using MultiTenant.Web.Helpers.Interfaces;

namespace MultiTenant.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ITenantHelper _tenantHelper;

        public OrderController(ITenantHelper tenantHelper) {
            this._tenantHelper = tenantHelper;
        }

        public IActionResult Index()
        {
            using (var ctx = new TenantDbContext(HttpContext, _tenantHelper))
            {
                ViewBag.Orders = ctx.Orders.ToList();
            }

            return View();
        }
    }
}