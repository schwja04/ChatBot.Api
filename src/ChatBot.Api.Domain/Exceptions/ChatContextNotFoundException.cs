using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Domain.Exceptions;

public class ChatContextNotFoundException : ApplicationException
{
    public ChatContextNotFoundException(Guid contextId)
    {
        ContextId = contextId;

        base.Data.Add(nameof(ContextId), contextId);
    }

    public Guid ContextId { get; }

    public override string Message => $"{nameof(ChatContext)} not found";
}

