namespace IdentityService.Api.Configuration;

public record JwtSettings
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; } 
}