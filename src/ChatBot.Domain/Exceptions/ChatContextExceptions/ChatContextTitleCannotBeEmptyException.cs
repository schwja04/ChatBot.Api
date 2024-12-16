namespace ChatBot.Api.Domain.Exceptions.ChatContextExceptions;

public sealed class ChatContextTitleCannotBeEmptyException(Guid contextId, string owner)
    : DomainException("Chat context title cannot be empty")
{
    public Guid ContextId { get; } = contextId;

    public string Owner { get; } = owner;
}