using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.Exceptions;
using MediatR;

namespace ChatBot.Api.Application.Commands.UpdateChatContextTitle;

internal class UpdateChatContextTitleCommandHandler(IChatContextRepository chatContextRepository) 
    : IRequestHandler<UpdateChatContextTitleCommand>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;

    public async Task Handle(UpdateChatContextTitleCommand request, CancellationToken cancellationToken)
    {
        var chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            throw new ChatContextNotFoundException(request.ContextId);
        }

        if (!string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new ChatContextAuthorizationException(request);
        }

        chatContext.SetTitle(request.Title);

        await _chatContextRepository.SaveAsync(chatContext, cancellationToken);
    }
}