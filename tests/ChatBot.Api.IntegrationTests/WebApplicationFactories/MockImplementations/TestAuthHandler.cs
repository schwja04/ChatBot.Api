using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ChatBot.Api.IntegrationTests.WebApplicationFactories.MockImplementations;

public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public static Guid UserId = Guid.NewGuid();
    
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("sub", UserId.ToString()),
            new Claim("preferred_username", "testuser"),
            new Claim("email", "testuser@test.com"),
        };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        
        var ticket = new AuthenticationTicket(principal, null, "Bearer");
        var result = AuthenticateResult.Success(ticket);
        
        return Task.FromResult(result);
    }
}