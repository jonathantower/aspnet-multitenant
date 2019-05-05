using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MultiTenant.Controllers
{
    public class ThemeController : Controller
    {
        [HttpGet("theme/t{tenantId}.css")]
        public IActionResult GetThemeCss(int tenantId)
        {
            return File($"/Themes/t{tenantId}.css", "text/css", $"t{tenantId}.css");
        }
    }
}