using System.Collections;

namespace ChatBot.Api.Application.Models.Exceptions;

public class ChatHistoryNotFoundException : ApplicationException
{
    public ChatHistoryNotFoundException(Guid contextId)
    {
        ContextId = contextId;

        base.Data.Add(nameof(ContextId), contextId);
    }

    public Guid ContextId { get; }

    public override string Message => "ChatHistory not found";
}

