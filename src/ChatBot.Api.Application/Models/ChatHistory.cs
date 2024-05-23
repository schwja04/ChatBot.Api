using System.Collections.ObjectModel;

namespace ChatBot.Api.Application.Models;

public record ChatHistory
{
    private readonly List<ChatMessage> _messages;

    private ChatHistory(Guid contextId, IEnumerable<ChatMessage> messages, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        ContextId = contextId;
        _messages = messages.ToList();

        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        ChatMessages = _messages.AsReadOnly();
    }

    public Guid ContextId { get; }

    public ReadOnlyCollection<ChatMessage> ChatMessages { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void AddMessage(ChatMessage message)
    {
        UpdatedAt = DateTimeOffset.Now;

        _messages.Add(message);
    }

    public static ChatHistory CreateNew()
    {
        DateTimeOffset now = DateTimeOffset.Now;

        return new ChatHistory(
            contextId: Guid.NewGuid(),
            messages: Enumerable.Empty<ChatMessage>(),
            createdAt: now,
            updatedAt: now);
    }

    public static ChatHistory CreateExisting(Guid contextId, IEnumerable<ChatMessage> messages, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        return new ChatHistory(
            contextId: contextId,
            messages: messages,
            createdAt: createdAt,
            updatedAt: updatedAt);
    }
}
