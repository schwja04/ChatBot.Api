using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Application.Models.Commands;

namespace ChatBot.Api.Application.Handlers;

public class ProcessChatMessageCommandHandler : IRequestHandler<ProcessChatMessageCommand, ProcessChatMessageCommandResponse>
{
    private readonly IChatCompletionRepository _chatCompletionRepository;
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public ProcessChatMessageCommandHandler(IChatCompletionRepository chatCompletionRepository, IChatHistoryRepository chatHistoryRepository)
    {
        _chatCompletionRepository = chatCompletionRepository;
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<ProcessChatMessageCommandResponse> Handle(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        ChatHistory? chatHistory = null;

        // Check if the context already exists
        if (request.ContextId != Guid.Empty)
        {
            chatHistory = await _chatHistoryRepository.GetChatHistoryAsync(request.ContextId, cancellationToken);

            if (chatHistory is null)
            {
                throw new ArgumentException("Invalid ContextId");
            }
        }

        // This will create a new context if the ContextId is Guid.Empty
        chatHistory ??= ChatHistory.CreateNew();

        ChatMessage userMessage = ChatMessage.CreateUserMessage(request.Content);
        chatHistory.AddMessage(userMessage);

        // Send the chat history to openai and save the response to the chat history
        ChatMessage assistantMessage = await _chatCompletionRepository.GetChatCompletionAsync(chatHistory, cancellationToken);
        chatHistory.AddMessage(assistantMessage);

        await _chatHistoryRepository.SaveChatHistoryAsync(chatHistory, cancellationToken);

        return new ProcessChatMessageCommandResponse
        {
            ContextId = chatHistory.ContextId,
            ChatMessage = assistantMessage
        };
    }
}

