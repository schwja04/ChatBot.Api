using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetManyChatContextMetadata;

internal class GetManyChatContextMetadataQueryHandler(IChatContextRepository chatContextRepository)
    : IRequestHandler<GetManyChatContextMetadataQuery, ReadOnlyCollection<ChatContextMetadata>>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;
    
    public async Task<ReadOnlyCollection<ChatContextMetadata>> Handle(GetManyChatContextMetadataQuery request, CancellationToken cancellationToken)
    {
        return await _chatContextRepository.GetManyMetadataAsync(request.Username, cancellationToken);
    }
}
