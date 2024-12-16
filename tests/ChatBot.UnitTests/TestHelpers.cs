using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.UnitTests;

public static class TestHelpers
{
    public static ControllerContext CreateTestControllerContext(string username)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new GenericPrincipal(new GenericIdentity(username), roles: null)
            }
        };
    }
}