using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.Exceptions;
using MediatR;

namespace ChatBot.Api.Application.Commands.DeleteChatContext;

internal class DeleteChatContextCommandHandler(IChatContextRepository chatContextRepository) 
    : IRequestHandler<DeleteChatContextCommand>
{
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;

    public async Task Handle(DeleteChatContextCommand request, CancellationToken cancellationToken)
    {
        ChatContext? chatContext = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (chatContext is null)
        {
            return;
        }

        if (!string.Equals(chatContext.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new ChatContextAuthorizationException(request);
        }

        await _chatContextRepository.DeleteAsync(request.ContextId, cancellationToken);
    }
}

