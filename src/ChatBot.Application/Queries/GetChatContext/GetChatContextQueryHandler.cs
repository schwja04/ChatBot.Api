using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.Exceptions.ChatContextExceptions;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetChatContext;

internal class GetChatContextQueryHandler(IChatContextRepository chatContextRepository) : IRequestHandler<GetChatContextQuery, ChatContext>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;

    public async Task<ChatContext> Handle(GetChatContextQuery request, CancellationToken cancellationToken)
    {
        var chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            throw new ChatContextNotFoundException(request.ContextId, request.Username);
        }

        if (!string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new ChatContextAuthorizationException(request.ContextId, request.Username);
        }

        return chatContext;
    }
}
