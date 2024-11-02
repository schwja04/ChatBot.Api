using System.Collections.ObjectModel;

namespace ChatBot.Api.Domain.ChatContextEntity;

public record ChatContext
{
    private readonly List<ChatMessage> _messages = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ChatContext() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    private ChatContext(
        Guid contextId, 
        string title, 
        string username, 
        IEnumerable<ChatMessage> messages, 
        DateTimeOffset createdAt, 
        DateTimeOffset updatedAt)
    {
        ContextId = contextId;
        Title = title;
        Username = username;
        _messages.AddRange(messages);

        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid ContextId { get; }

    public string Title { get; private set; }

    public string Username { get; }

    public ReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void SetTitle(string title)
    {
        UpdatedAt = DateTimeOffset.Now;

        Title = title;
    }

    public void AddMessage(ChatMessage message)
    {
        UpdatedAt = DateTimeOffset.Now;

        _messages.Add(message);
    }

    public static ChatContext CreateNew(string username)
    {
        DateTimeOffset now = DateTimeOffset.Now;

        return new ChatContext(
            contextId: Guid.NewGuid(),
            title: string.Empty,
            username: username,
            messages: Enumerable.Empty<ChatMessage>(),
            createdAt: now,
            updatedAt: now);
    }

    public static ChatContext CreateExisting(
        Guid contextId,
        string title,
        string username,
        IEnumerable<ChatMessage> messages,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        return new ChatContext(
            contextId: contextId,
            title: title,
            username: username,
            messages: messages,
            createdAt: createdAt,
            updatedAt: updatedAt);
    }
}
