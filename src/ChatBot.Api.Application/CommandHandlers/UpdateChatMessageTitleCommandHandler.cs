using MediatR;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Domain.Exceptions;
using ChatBot.Api.Domain.ChatHistoryEntity;

namespace ChatBot.Api.Application.CommandHandlers;

internal class UpdateChatMessageTitleCommandHandler : IRequestHandler<UpdateChatMessageTitleCommand>
{
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public UpdateChatMessageTitleCommandHandler(IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task Handle(UpdateChatMessageTitleCommand request, CancellationToken cancellationToken)
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
