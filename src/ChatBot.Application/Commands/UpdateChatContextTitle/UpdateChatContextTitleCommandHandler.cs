using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.Exceptions.ChatContextExceptions;
using MediatR;

namespace ChatBot.Application.Commands.UpdateChatContextTitle;

internal class UpdateChatContextTitleCommandHandler(IChatContextRepository chatContextRepository) 
    : IRequestHandler<UpdateChatContextTitleCommand>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;

    public async Task Handle(UpdateChatContextTitleCommand request, CancellationToken cancellationToken)
    {
        var chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            throw new ChatContextNotFoundException(request.ContextId, request.Username);
        }

        if (!string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new ChatContextAuthorizationException(request.ContextId, request.Username);
        }

        chatContext.SetTitle(request.Title);

        await _chatContextRepository.SaveAsync(chatContext, cancellationToken);
    }
}
