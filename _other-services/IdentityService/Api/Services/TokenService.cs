using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityService.Api.Configuration;
using IdentityService.Domain;
using IdentityService.Domain.AppUserAggregate;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Api.Services;

public class TokenService(IOptions<JwtSettings> jwtSettings)
{
    private readonly JwtSettings _configuration = jwtSettings.Value;

    public Task<AccessToken> GenerateTokenAsync(AppUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);
        
        var accessToken = new AccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresOn = new DateTimeOffset(token.ValidTo, TimeSpan.Zero),
            ExpiresIn = (long)TimeSpan.FromMinutes(30).TotalSeconds,
            RefreshToken = string.Empty
        };

        return Task.FromResult(accessToken);
    }
}