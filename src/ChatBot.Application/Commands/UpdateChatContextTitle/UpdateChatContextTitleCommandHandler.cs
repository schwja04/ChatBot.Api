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
                "Attempted to update title for chat context ({ContextId}) for user {Username}, but the chat context was not found",
                request.ContextId,
                request.Username);
            throw new ChatContextNotFoundException(request.ContextId, request.Username);
        }

        if (!string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(
                "Attempted to update title for chat context ({ContextId}) for user {Username}, but the user is not authorized",
                request.ContextId,
                request.Username);
            throw new ChatContextAuthorizationException(request.ContextId, request.Username);
        }


        _logger.LogInformation(
            "Updated title for chat context ({ContextId}) for user {Username}",
            chatContext.ContextId,
            chatContext.Username);
        chatContext.SetTitle(request.Title);
        await _chatContextRepository.SaveAsync(chatContext, cancellationToken);
    }
}
