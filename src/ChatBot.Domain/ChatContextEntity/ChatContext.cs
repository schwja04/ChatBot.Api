using System.Collections.ObjectModel;
using ChatBot.Domain.Exceptions.ChatContextExceptions;

namespace ChatBot.Domain.ChatContextEntity;

public record ChatContext
{
    private readonly List<ChatMessage> _messages = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ChatContext() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    private ChatContext(
        Guid contextId, 
        string title, 
        Guid userId, 
        IEnumerable<ChatMessage> messages, 
        DateTimeOffset createdAt, 
        DateTimeOffset updatedAt)
    {
        ContextId = contextId;
        Title = title;
        UserId = userId;
        _messages.AddRange(messages);

        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid ContextId { get; }

    public string Title { get; private set; }

    public Guid UserId { get; }

    public ReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ChatContextTitleCannotBeEmptyException(ContextId, UserId);
        }
        
        if (string.Equals(Title, title, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        UpdatedAt = DateTimeOffset.Now;
        Title = title;
    }

    public void AddMessage(ChatMessage message)
    {
        UpdatedAt = DateTimeOffset.Now;

        _messages.Add(message);
    }

    public static ChatContext CreateNew(Guid userId)
    {
        DateTimeOffset now = DateTimeOffset.Now;

        return new ChatContext(
            contextId: Guid.NewGuid(),
            title: string.Empty,
            userId: userId,
            messages: [],
            createdAt: now,
            updatedAt: now);
    }

    public static ChatContext CreateExisting(
        Guid contextId,
        string title,
        Guid userId,
        IEnumerable<ChatMessage> messages,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        return new ChatContext(
            contextId: contextId,
            title: title,
            userId: userId,
            messages: messages,
            createdAt: createdAt,
            updatedAt: updatedAt);
    }
}
