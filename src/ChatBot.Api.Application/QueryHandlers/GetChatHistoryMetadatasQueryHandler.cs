using MediatR;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Domain.ChatHistoryEntity;

namespace ChatBot.Api.Application.QueryHandlers;

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
