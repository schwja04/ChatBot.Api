using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.Exceptions.ChatContextExceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Commands.DeleteChatContext;

internal class DeleteChatContextCommandHandler(
    ILogger<DeleteChatContextCommandHandler> logger,
    IChatContextRepository chatContextRepository) 
    : IRequestHandler<DeleteChatContextCommand>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;
    private readonly ILogger<DeleteChatContextCommandHandler> _logger = logger;
    
    public async Task Handle(DeleteChatContextCommand request, CancellationToken cancellationToken)
    {
        ChatContext? chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            _logger.LogError(
                "Attempt to delete a chat context with id {ContextId} by user {Username} failed. Context not found.",
                request.ContextId,
                request.Username);
            throw new ChatContextNotFoundException(request.ContextId, request.Username);
        }

        if (!string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(
                "Attempt to delete a chat context with id {ContextId} by user {Username} failed. User is not authorized.",
                request.ContextId,
                request.Username);
            throw new ChatContextAuthorizationException(request.ContextId, request.Username);
        }

        _logger.LogInformation(
            "User {Username} is deleting chat context with id {ContextId}.",
            request.Username,
            request.ContextId);
        await _chatContextRepository.DeleteAsync(request.ContextId, cancellationToken);
    }
}

