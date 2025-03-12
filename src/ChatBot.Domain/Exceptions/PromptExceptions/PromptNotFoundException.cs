namespace ChatBot.Domain.Exceptions.PromptExceptions;

public sealed class PromptNotFoundException : ApplicationException
{
    public PromptNotFoundException(Guid promptId, Guid ownerId)
        : base($"Prompt not found.")
    {
        PromptId = promptId;
        OwnerId = ownerId;
        
        Data.Add(nameof(promptId), promptId);
        Data.Add(nameof(ownerId), ownerId);
    }
    
    public Guid PromptId { get; }
    public Guid OwnerId { get; }
}