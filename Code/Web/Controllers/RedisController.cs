using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiTenant.Helpers.Interfaces;

namespace MultiTenant.Controllers
{
    public class RedisController : Controller
    {
        private readonly IRedisHelper _redis;
        
        public RedisController(IRedisHelper redis)
        {
            _redis = redis;
        }

        public IActionResult Index()
        {
            var cacheKey = "TheTime";
            var existingTime = _redis.Get(cacheKey);
            if (!string.IsNullOrEmpty(existingTime))
            {
                ViewBag.Message = "Fetched from cache : " + existingTime;
            }
            else
            {
                existingTime = DateTime.Now.ToString();
                _redis.Set(cacheKey, existingTime, expirationInSeconds: 30);
                ViewBag.Message = "Added to cache : " + existingTime;
            }

            return View();
        }
    }
}