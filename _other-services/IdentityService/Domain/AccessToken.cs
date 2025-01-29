namespace IdentityService.Domain;

public record AccessToken
{
    public required string Token { get; init; }
    public required DateTimeOffset ExpiresOn { get; init; }
    public required long ExpiresIn { get; init; }
    public required string RefreshToken { get; init; }
}