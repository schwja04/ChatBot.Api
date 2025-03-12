namespace ChatBot.Domain.ChatContextEntity;

public record ChatContextMetadata
{
    private ChatContextMetadata(
        Guid contextId, string title, Guid userId, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        ContextId = contextId;
        Title = title;
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid ContextId { get; }

    public string Title { get; }

    public Guid UserId { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; }

    public static ChatContextMetadata CreateExisting(
        Guid contextId, string title, Guid userId, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        return new ChatContextMetadata(
            contextId: contextId,
            title: title,
            userId: userId,
            createdAt: createdAt,
            updatedAt: updatedAt);
    }
}

