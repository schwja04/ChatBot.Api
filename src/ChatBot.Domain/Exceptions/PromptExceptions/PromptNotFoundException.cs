namespace ChatBot.Domain.Exceptions.PromptExceptions;

public sealed class PromptNotFoundException : ApplicationException
{
    public PromptNotFoundException(Guid promptId, string owner)
        : base($"Prompt not found.")
    {
        PromptId = promptId;
        Owner = owner;
        
        Data.Add(nameof(promptId), promptId);
        Data.Add(nameof(owner), owner);
    }
    
    public Guid PromptId { get; }
    public string Owner { get; }
}