namespace ChatBot.Api.Contracts;

public record ChatMessageResponse
{
    public required Guid MessageId { get; init; }

    public required string Content { get; init; } = null!;

    public required DateTimeOffset CreatedAt { get; init; }

    public required string Role { get; init; } = null!;
}
