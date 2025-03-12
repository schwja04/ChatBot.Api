namespace ChatBot.Domain.Exceptions.ChatContextExceptions;

public sealed class ChatContextAuthorizationException : ApplicationException
{
	public ChatContextAuthorizationException(Guid contextId, Guid unauthorizedUserId)
		: base("User is not authorized to access this ChatContext")
	{
		Data.Add(nameof(contextId), contextId);
		Data.Add(nameof(unauthorizedUserId), unauthorizedUserId);
		
		ContextId = contextId;
		UnauthorizedUserId = unauthorizedUserId;
	}
	
	public Guid ContextId { get; }
	public Guid UnauthorizedUserId { get; }
}

