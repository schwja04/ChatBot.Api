using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Domain.Exceptions;

public class ChatContextAuthorizationException : ApplicationException
{
	public ChatContextAuthorizationException(object	request)
	{
		base.Data.Add(nameof(request), request);
	}

    public override string Message => $"User is not authorized to access this {nameof(ChatContext)}";
}

