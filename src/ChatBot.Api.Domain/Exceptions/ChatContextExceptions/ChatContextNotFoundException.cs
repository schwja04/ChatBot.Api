using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Domain.Exceptions.ChatContextExceptions;

public sealed class ChatContextNotFoundException : ApplicationException
{
    public ChatContextNotFoundException(Guid contextId) : base($"{nameof(ChatContext)} not found")
    {
        ContextId = contextId;
        Data.Add(nameof(ContextId), contextId);
    }

    public Guid ContextId { get; }
}

