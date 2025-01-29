namespace IdentityService.Api.Contracts;

public record MyRegisterRequest
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}