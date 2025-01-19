namespace ChatBot.Domain.ChatContextEntity;

public record ChatContextMetadata
{
    private ChatContextMetadata(
        Guid contextId, string title, string username, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        ContextId = contextId;
        Title = title;
        Username = username;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid ContextId { get; }

    public string Title { get; }

    public string Username { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; }

    public static ChatContextMetadata CreateExisting(
        Guid contextId, string title, string username, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        return new ChatContextMetadata(
            contextId: contextId,
            title: title,
            username: username,
            createdAt: createdAt,
            updatedAt: updatedAt);
    }
}

