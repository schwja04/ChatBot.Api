using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.UnitTests;

public static class TestHelpers
{
    public static ControllerContext CreateTestControllerContext(Guid userId)
    {
        var claims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim("preferred_username", "testuser"),
            new Claim("email", "testuser@test.com"),
        };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = principal
            }
        };
    }
}