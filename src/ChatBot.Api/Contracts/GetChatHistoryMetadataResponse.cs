namespace ChatBot.Api.Contracts;

public record GetChatHistoryMetadataResponse
{
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }

    public required string Username { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset UpdatedAt { get; init; }
}
