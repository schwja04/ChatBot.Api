using MediatR;
using ChatBot.Api.Domain.Exceptions;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Application.QueryHandlers;

internal class GetChatContextQueryHandler(IChatContextRepository chatContextRepository) : IRequestHandler<GetChatContextQuery, GetChatContextQueryResponse?>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;

    public async Task<GetChatContextQueryResponse?> Handle(GetChatContextQuery request, CancellationToken cancellationToken)
    {
        var chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            return null;
        }

        if (string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            return new GetChatContextQueryResponse
            {
                ChatContext = chatContext
            };
        }

        throw new ChatContextAuthorizationException(request);
    }
}
