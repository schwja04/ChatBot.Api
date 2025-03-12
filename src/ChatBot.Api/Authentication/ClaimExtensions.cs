using System.Security.Claims;

namespace ChatBot.Api.Authentication;

internal static class ClaimExtensions
{
    public static string GetUsername(this ClaimsPrincipal principal)
    {
        return principal
            .Claims
            .First(claim => string.Equals(claim.Type, "preferred_username", StringComparison.OrdinalIgnoreCase))
            .Value;
    }
    
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal
            .Claims
            .FirstOrDefault(claim => string.Equals(claim.Type, "email", StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }
    
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        return Guid.Parse(principal
            .Claims
            .First(claim => string.Equals(claim.Type, "sub", StringComparison.OrdinalIgnoreCase))
            .Value);
    }
}