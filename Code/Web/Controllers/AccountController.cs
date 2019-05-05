using JwtAuthenticationHelper.Abstractions;
using MultiTenant.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MultiTenant.Web.Options;
using MultiTenant.Web.Helpers.Interfaces;

namespace MultiTenant.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IJwtTokenGenerator tokenGenerator;
        private readonly IUserHelper _userHelper;
        private readonly DatabaseOptions _databaseOptions;

        public AccountController(IJwtTokenGenerator tokenGenerator,
            IOptions<DatabaseOptions> databaseOptions,
            IUserHelper userHelper)
        {
            this.tokenGenerator = tokenGenerator;
            this._userHelper = userHelper;
            this._databaseOptions = databaseOptions.Value;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserCredentials userCredentials, string returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;
            var returnTo = "/Account/Login";

            // Replace this with your custom authentication logic which will
            // securely return the authenticated user's details including
            // any role specific info

            if (_userHelper.ValidateUser(userCredentials.Username, userCredentials.Password))
            {
                var user = _userHelper.GetUser(userCredentials.Username);

                var userInfo = new UserInfo
                {
                    FirstName = "Firstname",
                    LastName = "Lastname",
                    HasAdminRights = true,
                    TenantId = user.TenantId
                };

                var accessTokenResult = tokenGenerator.GenerateAccessTokenWithClaimsPrincipal(
                    userCredentials.Username,
                    AddMyClaims(userInfo));
                await HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal,
                    accessTokenResult.AuthProperties);
                returnTo = returnUrl;
            }

            return RedirectToLocal(returnTo);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private static IEnumerable<Claim> AddMyClaims(UserInfo authenticatedUser)
        {
            var myClaims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, authenticatedUser.FirstName),
                new Claim(ClaimTypes.Surname, authenticatedUser.LastName),
                new Claim("HasAdminRights", authenticatedUser.HasAdminRights ? "Y" : "N"),
                new Claim("Tenant", authenticatedUser.TenantId.ToString())
            };

            return myClaims;
        }
    }
}