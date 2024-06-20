using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Application.Models.Commands;
using ChatBot.Api.Application.Models.Exceptions;
using MediatR;

namespace ChatBot.Api.Application.Handlers.CommandHandlers;

internal class DeleteChatHistoryCommandHandler : IRequestHandler<DeleteChatHistoryCommand>
{
    private readonly IChatHistoryRepository _chatHistoryRepository;

	public DeleteChatHistoryCommandHandler(IChatHistoryRepository chatHistoryRepository)
	{
        _chatHistoryRepository = chatHistoryRepository;
	}

    public async Task Handle(DeleteChatHistoryCommand request, CancellationToken cancellationToken)
    {
        ChatHistory? chatHistory = await _chatHistoryRepository.GetChatHistoryAsync(request.ContextId, cancellationToken);

        if (chatHistory is null)
        {
            return;
        }

        if (!string.Equals(chatHistory.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new ChatHistoryAuthorizationException(request);
        }

        await _chatHistoryRepository.DeleteChatHistoryAsync(request.ContextId, cancellationToken);
    }
}

