using Microsoft.AspNetCore.Builder;
using System.Linq;

namespace MultiTenant.Web.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseQuerystringJwt(this IApplicationBuilder app)
        {
            const string QS_TOKEN_PARAM_NAME = "token";

            app.Use(async (context, next) =>
            {
                if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                {
                    if (context.Request.QueryString.HasValue)
                    {
                        var token = context.Request.QueryString.Value
                            .Split('&').SingleOrDefault(x => x.Contains(QS_TOKEN_PARAM_NAME))?.Split('=')[1];

                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                        }
                    }
                }
                await next.Invoke();
            });
        }
    }
}
