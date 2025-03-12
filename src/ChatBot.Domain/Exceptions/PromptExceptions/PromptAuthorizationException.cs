namespace ChatBot.Domain.Exceptions.PromptExceptions;

public class PromptAuthorizationException : ApplicationException
{
	public PromptAuthorizationException(Guid promptId, Guid unauthorizedUserId)
        : base("User is not authorized to access this Prompt")
    {
        base.Data.Add(nameof(promptId), promptId);
        base.Data.Add(nameof(unauthorizedUserId), unauthorizedUserId);

        PromptId = promptId;
        UnauthorizedUserId = unauthorizedUserId;
    }
    
    public Guid PromptId { get; }
    public Guid UnauthorizedUserId { get; }
}

