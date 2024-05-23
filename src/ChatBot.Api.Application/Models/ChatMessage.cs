namespace ChatBot.Api.Application.Models;

public record ChatMessage
{
    private ChatMessage(Guid messageId, string content, DateTimeOffset createdAt, ChatterRole role)
    {
        MessageId = messageId;
        Content = content;
        CreatedAt = createdAt;
        Role = role;
    }

    public Guid MessageId { get; }

    public string Content { get; }

    public DateTimeOffset CreatedAt { get; }

    public ChatterRole Role { get; }

    public static ChatMessage CreateUserMessage(string content)
    {
        return new ChatMessage(
            messageId: Guid.NewGuid(),
            content: content,
            createdAt: DateTimeOffset.UtcNow,
            role: ChatterRole.User);
    }

    public static ChatMessage CreateAssistantMessage(string content)
    {
        return new ChatMessage(
            messageId: Guid.NewGuid(),
            content: content,
            createdAt: DateTimeOffset.UtcNow,
            role: ChatterRole.Assistant);
    }

    public static ChatMessage CreateExistingChatMessage(Guid messageId, string content, DateTimeOffset createdAt, ChatterRole role)
    {
        return new ChatMessage(
            messageId: messageId,
            content: content,
            createdAt: createdAt,
            role: role);
    }
}
