using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace JwtAuthenticationHelper.Types
{
    public sealed class TokenWithClaimsPrincipal
    {
        public string AccessToken { get; set; }

        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        public AuthenticationProperties AuthProperties { get; set; }
    }
}