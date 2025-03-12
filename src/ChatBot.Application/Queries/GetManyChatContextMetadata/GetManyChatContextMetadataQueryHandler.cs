using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Queries.GetManyChatContextMetadata;

internal class GetManyChatContextMetadataQueryHandler(
    ILogger<GetManyChatContextMetadataQueryHandler> logger,
    IChatContextRepository chatContextRepository)
    : IRequestHandler<GetManyChatContextMetadataQuery, ReadOnlyCollection<ChatContextMetadata>>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;
    private readonly ILogger _logger = logger;
    
    public async Task<ReadOnlyCollection<ChatContextMetadata>> Handle(GetManyChatContextMetadataQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving chat context metadata records for user {UserId}",
            request.UserId);
        return await _chatContextRepository.GetManyMetadataAsync(request.UserId, cancellationToken);
    }
}
