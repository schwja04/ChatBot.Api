using MediatR;

using ChatBot.Api.Application.Models.Queries;
using ChatBot.Api.Application.Abstractions.Repositories;

namespace ChatBot.Api.Application.Handlers.QueryHandlers;

internal class GetChatHistoryMetadatasQueryHandler
    : IRequestHandler<GetChatHistoryMetadatasQuery, GetChatHistoryMetadatasQueryResponse>
{
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public GetChatHistoryMetadatasQueryHandler(IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<GetChatHistoryMetadatasQueryResponse> Handle(GetChatHistoryMetadatasQuery request, CancellationToken cancellationToken)
    {
        var chatHistoryMetadatas = await _chatHistoryRepository.GetChatHistoryMetadatasAsync(request.UserName, cancellationToken);

        return new GetChatHistoryMetadatasQueryResponse
        {
            ChatHistoryMetadatas = chatHistoryMetadatas
        };
    }
}
