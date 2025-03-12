namespace ChatBot.Domain.Exceptions.PromptExceptions;

public sealed class PromptKeyCannotBeEmptyException : DomainException
{
    public PromptKeyCannotBeEmptyException(Guid promptId, Guid ownerId) : base("Prompt key cannot be empty.")
    {
        PromptId = promptId;
        OwnerId = ownerId;
        Data.Add(nameof(promptId), promptId);
        Data.Add(nameof(ownerId), ownerId);
    }
    
    public Guid PromptId { get; }
    
    public Guid OwnerId { get; }
}