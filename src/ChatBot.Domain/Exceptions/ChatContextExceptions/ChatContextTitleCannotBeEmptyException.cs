namespace ChatBot.Domain.Exceptions.ChatContextExceptions;

public sealed class ChatContextTitleCannotBeEmptyException(Guid contextId, Guid userId)
    : DomainException("Chat context title cannot be empty")
{
    public Guid ContextId { get; } = contextId;

    public Guid Owner { get; } = userId;
}