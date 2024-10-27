using MediatR;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Application.QueryHandlers;

internal class GetManyChatContextMetadataQueryHandler(IChatContextRepository chatContextRepository)
    : IRequestHandler<GetManyChatContextMetadataQuery, GetChatHistoryMetadatasQueryResponse>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;
    
    public async Task<GetChatHistoryMetadatasQueryResponse> Handle(GetManyChatContextMetadataQuery request, CancellationToken cancellationToken)
    {
        var chatHistoryMetadatas = await _chatContextRepository.GetManyMetadataAsync(request.UserName, cancellationToken);

        return new GetChatHistoryMetadatasQueryResponse
        {
            ChatContextMetadatas = chatHistoryMetadatas
        };
    }
}
