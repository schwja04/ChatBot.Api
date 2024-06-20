using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Queries;
using ChatBot.Api.Application.Models.Exceptions;

namespace ChatBot.Api.Application.Handlers.QueryHandlers;

internal class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, GetChatHistoryQueryResponse?>
{
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public GetChatHistoryQueryHandler(IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<GetChatHistoryQueryResponse?> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var chatHistory = await _chatHistoryRepository.GetChatHistoryAsync(request.ContextId, cancellationToken);

        if (chatHistory is null)
        {
            return null;
        }

        if (string.Equals(chatHistory.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            return new GetChatHistoryQueryResponse
            {
                ChatHistory = chatHistory
            };
        }

        throw new ChatHistoryAuthorizationException(request);
    }
}
