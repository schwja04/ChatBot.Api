using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.Exceptions.ChatContextExceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Queries.GetChatContext;

internal class GetChatContextQueryHandler(
    ILogger<GetChatContextQueryHandler> logger,
    IChatContextRepository chatContextRepository) 
    : IRequestHandler<GetChatContextQuery, ChatContext>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;
    private readonly ILogger _logger = logger;

    public async Task<ChatContext> Handle(GetChatContextQuery request, CancellationToken cancellationToken)
    {
        var chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            _logger.LogError(
                "Attempted to get chat context ({ContextId}) for user {UserId}, but the chat context was not found",
                request.ContextId,
                request.UserId);
            throw new ChatContextNotFoundException(request.ContextId, request.UserId);
        }

        if (chatContext.UserId != request.UserId)
        {
            _logger.LogError(
                "Attempted to get chat context ({ContextId}) for user {UserId}, but the user is not authorized",
                request.ContextId,
                request.UserId);
            throw new ChatContextAuthorizationException(request.ContextId, request.UserId);
        }

        _logger.LogInformation(
            "Retrieved chat context ({ContextId}) for user {UserId}",
            chatContext.ContextId,
            chatContext.UserId);
        return chatContext;
    }
}
