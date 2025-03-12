namespace ChatBot.Api.Contracts;

public record GetChatContextMetadataResponse
{
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }

    public required Guid UserId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset UpdatedAt { get; init; }
}
