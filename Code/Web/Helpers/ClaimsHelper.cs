using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultiTenant.Helpers
{
    public static class ClaimsHelper
    {
        public static bool HasClaim(ClaimsPrincipal user, string claimName)
        {
            return user != null && user.Claims.Any(i => string.Compare(i.Type, claimName.ToLower(), StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static T GetClaim<T>(ClaimsPrincipal user, string claimName)
        {
            if (user == null)
            {
                return default;
            }

            var c = user.Claims.FirstOrDefault(i => string.Compare(i.Type, claimName.ToLower(), StringComparison.OrdinalIgnoreCase) == 0);
            if (c == null) return default;

            return (T)Convert.ChangeType(c.Value, typeof(T));
        }

        public static List<T> GetClaims<T>(ClaimsPrincipal user, string claimName)
        {
            if (user == null)
            {
                return null;
            }

            var claims = user.Claims.Where(
                c => string.Compare(c.Type, claimName.ToLower(), StringComparison.OrdinalIgnoreCase) == 0).ToList();
            if (!claims.Any())
            {
                return null;
            }
            return claims.Select(c =>
               (T)Convert.ChangeType(c.Value, typeof(T))).ToList();
        }

        public static void AddClaim(ClaimsPrincipal user, string claimName, string claimValueType, string val)
        {
            var c = user?.Claims.FirstOrDefault(i => string.Compare(i.Type, claimName.ToLower(), StringComparison.OrdinalIgnoreCase) == 0);
            if (c != null)
            {
                SetClaim(user, claimName, val);
            }
            else
            {
                var identity = user.Identities.First();
                identity.AddClaim(new Claim(claimName, val, claimValueType));
            }
        }

        public static void SetClaim(ClaimsPrincipal user, string claimName, string val)
        {
            var c = user?.Claims.FirstOrDefault(i => string.Compare(i.Type, claimName.ToLower(), StringComparison.OrdinalIgnoreCase) == 0);
            if (c == null)
            {
                return;
            }

            var identity = user.Identities.First();

            identity.RemoveClaim(c);
            identity.AddClaim(new Claim(c.Type, val, c.ValueType));
        }
    }
}
