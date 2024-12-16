namespace ChatBot.Domain.ChatContextEntity;
public record ChatMessage
{
    private ChatMessage(
        Guid messageId, string content, string promptKey, DateTimeOffset createdAt, ChatterRole role)
    {
        MessageId = messageId;
        Content = content;
        PromptKey = promptKey;
        CreatedAt = createdAt;
        Role = role;
    }

    public Guid MessageId { get; }

    public string Content { get; }

    public DateTimeOffset CreatedAt { get; }

    public ChatterRole Role { get; }

    public string PromptKey { get; }

    public static ChatMessage CreateUserMessage(string content, string promptKey)
    {
        return new ChatMessage(
            messageId: Guid.NewGuid(),
            content: content,
            promptKey: promptKey,
            createdAt: DateTimeOffset.UtcNow,
            role: ChatterRole.User);
    }

    public static ChatMessage CreateAssistantMessage(string content)
    {
        return new ChatMessage(
            messageId: Guid.NewGuid(),
            content: content,
            promptKey: PromptEntity.PromptKey.None,
            createdAt: DateTimeOffset.UtcNow,
            role: ChatterRole.Assistant);
    }

    public static ChatMessage CreateExistingChatMessage(
        Guid messageId, string content, string promptKey, DateTimeOffset createdAt, ChatterRole role)
    {
        return new ChatMessage(
            messageId: messageId,
            content: content,
            promptKey: promptKey,
            createdAt: createdAt,
            role: role);
    }
}
