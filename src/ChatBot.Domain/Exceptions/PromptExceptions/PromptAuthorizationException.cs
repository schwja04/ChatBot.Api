namespace ChatBot.Domain.Exceptions.PromptExceptions;

public class PromptAuthorizationException : ApplicationException
{
	public PromptAuthorizationException(Guid promptId, string unauthorizedUser)
        : base("User is not authorized to access this Prompt")
    {
        base.Data.Add(nameof(promptId), promptId);
        base.Data.Add(nameof(unauthorizedUser), unauthorizedUser);

        PromptId = promptId;
        UnauthorizedUser = unauthorizedUser;
    }
    
    public Guid PromptId { get; }
    public string UnauthorizedUser { get; }
}

