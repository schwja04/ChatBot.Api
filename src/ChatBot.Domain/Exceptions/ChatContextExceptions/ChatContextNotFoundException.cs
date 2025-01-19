using ChatBot.Domain.ChatContextEntity;

namespace ChatBot.Domain.Exceptions.ChatContextExceptions;

public sealed class ChatContextNotFoundException : ApplicationException
{
    public ChatContextNotFoundException(Guid contextId, string username) : base($"{nameof(ChatContext)} not found")
    {
        ContextId = contextId;
        Username = username;
        Data.Add(nameof(ContextId), contextId);
        Data.Add(nameof(username), username);
    }

    public Guid ContextId { get; }
    public string Username { get; }
}

