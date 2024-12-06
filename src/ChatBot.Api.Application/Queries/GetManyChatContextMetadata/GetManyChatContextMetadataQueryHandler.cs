using ChatBot.Api.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetManyChatContextMetadata;

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
