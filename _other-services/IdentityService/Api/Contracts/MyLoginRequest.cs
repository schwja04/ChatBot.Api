namespace IdentityService.Api.Contracts;

public record MyLoginRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}