using ChatBot.Domain.ChatContextEntity;

namespace ChatBot.Domain.Exceptions.ChatContextExceptions;

public sealed class ChatContextNotFoundException : ApplicationException
{
    public ChatContextNotFoundException(Guid contextId, Guid userId) : base($"{nameof(ChatContext)} not found")
    {
        ContextId = contextId;
        UserId = userId;
        Data.Add(nameof(ContextId), contextId);
        Data.Add(nameof(UserId), userId);
    }

    public Guid ContextId { get; }
    public Guid UserId { get; }
}

