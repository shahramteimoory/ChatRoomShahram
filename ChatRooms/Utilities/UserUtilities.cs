using System.Security.Claims;

namespace ChatRooms.Utilities
{
    public static class UserUtilities
    {
        public static long GetUserId(this ClaimsPrincipal? claims)
        {
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Convert.ToInt64(userId);
        }
        public static string GetUserName(this ClaimsPrincipal? claims)
        {
            var UserName = claims.FindFirst(ClaimTypes.Name).Value;
            return UserName;
        }
    }
}
