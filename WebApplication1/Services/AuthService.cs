using System.Security.Claims;

namespace WebApplication1.Services
{
    public class AuthService
    {
        public string GetClaimValue(HttpContext httpContext, string valueType)
        {
            if (string.IsNullOrEmpty(valueType)) return null;
            var identity = httpContext.User.Identity as ClaimsIdentity;
            var valueObj = identity == null ? null : identity.Claims.FirstOrDefault(x => x.Type == valueType);
            return valueObj == null ? null : valueObj.Value;
        }
    }
}
