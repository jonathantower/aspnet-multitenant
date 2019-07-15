using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenant.Web.Data;
using MultiTenant.Web.Helpers.Interfaces;
using MultiTenant.Web.Repositories.Interfaces;

namespace MultiTenant.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ITenantHelper _tenantHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(ITenantHelper tenantHelper, 
            IOrderRepository orderRepository,
            IHttpContextAccessor httpContextAccessor) {
            this._tenantHelper = tenantHelper;
            this._orderRepository = orderRepository;
            this._httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            using (var ctx = new TenantDbContext(_httpContextAccessor, _tenantHelper))
            {
                ViewBag.Orders = ctx.Orders.ToList();
                //ViewBag.Orders = _orderRepository.GetAll().ToList();
            }

            return View();
        }

        public IActionResult Test()
        {
            var order = _orderRepository.GetAll().First();

            // uh oh!
            order.TenantId++;

            _orderRepository.Update(order);

            return View("Index");
        }
    }
}