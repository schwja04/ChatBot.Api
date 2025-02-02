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
                "Attempted to get chat context ({ContextId}) for user {Username}, but the chat context was not found",
                request.ContextId,
                request.Username);
            throw new ChatContextNotFoundException(request.ContextId, request.Username);
        }

        if (!string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(
                "Attempted to get chat context ({ContextId}) for user {Username}, but the user is not authorized",
                request.ContextId,
                request.Username);
            throw new ChatContextAuthorizationException(request.ContextId, request.Username);
        }

        _logger.LogInformation(
            "Retrieved chat context ({ContextId}) for user {Username}",
            chatContext.ContextId,
            chatContext.Username);
        return chatContext;
    }
}
