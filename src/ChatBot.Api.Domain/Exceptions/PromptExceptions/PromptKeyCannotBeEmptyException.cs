namespace ChatBot.Api.Domain.Exceptions.PromptExceptions;

public sealed class PromptKeyCannotBeEmptyException : DomainException
{
    public PromptKeyCannotBeEmptyException(Guid promptId, string owner) : base("Prompt key cannot be empty.")
    {
        PromptId = promptId;
        Owner = owner;
        Data.Add(nameof(promptId), promptId);
        Data.Add(nameof(owner), owner);
    }
    
    public Guid PromptId { get; }
    
    public string Owner { get; }
}