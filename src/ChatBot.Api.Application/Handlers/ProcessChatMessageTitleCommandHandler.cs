using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Commands;
using ChatBot.Api.Application.Models.Exceptions;

namespace ChatBot.Api.Application.Handlers;

public class ProcessChatMessageTitleCommandHandler : IRequestHandler<ProcessChatMessageTitleCommand>
{
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public ProcessChatMessageTitleCommandHandler(IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task Handle(ProcessChatMessageTitleCommand request, CancellationToken cancellationToken)
    {
        var chatHistory = await _chatHistoryRepository.GetChatHistoryAsync(request.ContextId, cancellationToken);

        if (chatHistory is null)
        {
            throw new ChatHistoryNotFoundException(request.ContextId);
        }

        if (!string.Equals(chatHistory.Username, request.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new ChatHistoryAuthorizationException(request);
        }

        chatHistory.SetTitle(request.Title);

        await _chatHistoryRepository.SaveChatHistoryAsync(chatHistory, cancellationToken);
    }
}
