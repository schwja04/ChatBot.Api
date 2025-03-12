using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.Exceptions.ChatContextExceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Commands.UpdateChatContextTitle;

internal class UpdateChatContextTitleCommandHandler(
    ILogger<UpdateChatContextTitleCommandHandler> logger,
    IChatContextRepository chatContextRepository) 
    : IRequestHandler<UpdateChatContextTitleCommand>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;
    private readonly ILogger _logger = logger;

    public async Task Handle(UpdateChatContextTitleCommand request, CancellationToken cancellationToken)
    {
        var chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            _logger.LogError(
                "Attempted to update title for chat context ({ContextId}) for user ({UserId}), but the chat context was not found",
                request.ContextId,
                request.UserId);
            throw new ChatContextNotFoundException(request.ContextId, request.UserId);
        }

        if (chatContext.UserId != request.UserId)
        {
            _logger.LogError(
                "Attempted to update title for chat context ({ContextId}) for user {UserId}, but the user is not authorized",
                request.ContextId,
                request.UserId);
            throw new ChatContextAuthorizationException(request.ContextId, request.UserId);
        }


        _logger.LogInformation(
            "Updated title for chat context ({ContextId}) for user {UserId}",
            chatContext.ContextId,
            chatContext.UserId);
        chatContext.SetTitle(request.Title);
        await _chatContextRepository.SaveAsync(chatContext, cancellationToken);
    }
}
