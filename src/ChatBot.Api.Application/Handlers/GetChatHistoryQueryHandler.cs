using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Queries;

namespace ChatBot.Api.Application.Handlers;

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, GetChatHistoryQueryResponse?>
{
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public GetChatHistoryQueryHandler(IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<GetChatHistoryQueryResponse?> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var chatHistory = await _chatHistoryRepository.GetChatHistoryAsync(request.ContextId, cancellationToken);

        if (chatHistory is not null)
        {
            return new GetChatHistoryQueryResponse
            {
                ChatHistory = chatHistory
            };
        }

        return (GetChatHistoryQueryResponse?)null;
    }
}
