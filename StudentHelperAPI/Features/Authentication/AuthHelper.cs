using System.Security.Claims;
namespace StudentHelperAPI.Features.Authentication
{
    public static class AuthHelper
    {
        public static string GetCurrentRole(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return string.Empty;

            var role = user.FindFirst(ClaimTypes.Role)?.Value
                    ?? user.FindFirst("role")?.Value
                    ?? user.FindFirst("Role")?.Value
                    ?? user.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            return role ?? string.Empty;
        }

        public static Guid GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return Guid.Parse("00000000-0000-0000-0000-000000000001"); ;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? user.FindFirst("Id")?.Value;

            return Guid.TryParse(userIdClaim, out Guid userId) ? userId :
                Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
        public static Guid GetCurrentAuthor_id(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return Guid.Parse("00000000-0000-0000-0000-000000000001"); ;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? user.FindFirst("author_id")?.Value;

            return Guid.TryParse(userIdClaim, out Guid userId) ? userId :
                Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }
}


