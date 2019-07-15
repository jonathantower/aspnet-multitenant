using JwtAuthenticationHelper.Data;
using JwtAuthenticationHelper.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MultiTenant.Helpers;
using MultiTenant.Helpers.Interfaces;
using MultiTenant.Web.Data;
using MultiTenant.Web.Extensions;
using MultiTenant.Web.Helpers;
using MultiTenant.Web.Helpers.Interfaces;
using MultiTenant.Web.Models;
using MultiTenant.Web.Options;
using MultiTenant.Web.Repositories;
using MultiTenant.Web.Repositories.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace MultiTenant.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            HandleJwt(services);
            HandleDepencencyInjection(services);
            HandleOptions(services);

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = "127.0.0.1";
                //option.InstanceName = "master";
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseQuerystringJwt();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void HandleOptions(IServiceCollection services)
        {
            services.Configure<DatabaseOptions>(Configuration.GetSection("Database"));
        }

        private void HandleJwt(IServiceCollection services)
        {
            // retrieve the configured token params and establish a TokenValidationParameters object,
            // we are going to need this later.
            var validationParams = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,

                ValidateAudience = true,
                ValidAudience = Configuration["Token:Audience"],

                ValidateIssuer = true,
                ValidIssuer = Configuration["Token:Issuer"],

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Token:SigningKey"])),
                ValidateIssuerSigningKey = true,

                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            services.AddJwtAuthenticationWithProtectedCookie(validationParams);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequiresAdmin", policy => policy.RequireClaim("HasAdminRights"));
            });
        }

        private static void HandleDepencencyInjection(IServiceCollection services)
        {
            services.AddScoped<SharedDbContext, SharedDbContext>();
            services.AddScoped<TenantDbContext, TenantDbContext>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped<IRedisHelper, RedisHelper>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<ITenantHelper, TenantHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}