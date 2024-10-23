namespace ChatBot.Api.Domain.Exceptions;

public class ChatHistoryAuthorizationException : ApplicationException
{
	public ChatHistoryAuthorizationException(object	request)
	{
		base.Data.Add(nameof(request), request);
	}

    public override string Message => "User is not authorized to access this ChatHistory";
}

