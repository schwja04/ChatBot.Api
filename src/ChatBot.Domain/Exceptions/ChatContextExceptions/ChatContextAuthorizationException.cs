namespace ChatBot.Api.Domain.Exceptions.ChatContextExceptions;

public sealed class ChatContextAuthorizationException : ApplicationException
{
	public ChatContextAuthorizationException(Guid contextId, string unauthorizedUser)
		: base("User is not authorized to access this ChatContext")
	{
		Data.Add(nameof(contextId), contextId);
		Data.Add(nameof(unauthorizedUser), unauthorizedUser);
		
		ContextId = contextId;
		UnauthorizedUser = unauthorizedUser;
	}
	
	public Guid ContextId { get; }
	public string UnauthorizedUser { get; }
}

