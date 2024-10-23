using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Domain.Exceptions;
using ChatBot.Api.Domain.ChatHistoryEntity;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Application.CommandHandlers;

internal class ProcessChatMessageCommandHandler : IRequestHandler<ProcessChatMessageCommand, ProcessChatMessageCommandResponse>
{
    private readonly IChatCompletionRepository _chatCompletionRepository;
    private readonly IChatHistoryRepository _chatHistoryRepository;

    public ProcessChatMessageCommandHandler(
        IChatCompletionRepository chatCompletionRepository,
        IChatHistoryRepository chatHistoryRepository)
    {
        _chatCompletionRepository = chatCompletionRepository;
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<ProcessChatMessageCommandResponse> Handle(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        ChatHistory chatHistory = await GetChatHistoryAsync(request, cancellationToken);

        if (string.IsNullOrWhiteSpace(chatHistory.Title))
        {
            string title = await GenerateTitleAsync(request, cancellationToken);
            chatHistory.SetTitle(title);
        }

        ChatMessage userMessage = ChatMessage.CreateUserMessage(request.Content, request.PromptKey);
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

    private async Task<ChatHistory> GetChatHistoryAsync(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        ChatHistory? savedChatHistory = null;

        // Check if the context already exists
        if (request.ContextId != Guid.Empty)
        {
            savedChatHistory = await _chatHistoryRepository.GetChatHistoryAsync(request.ContextId, cancellationToken);

            if (savedChatHistory is null)
            {
                throw new ChatHistoryNotFoundException(request.ContextId);
            }

            if (!string.Equals(savedChatHistory.Username, request.Username, StringComparison.OrdinalIgnoreCase))
            {
                throw new ChatHistoryAuthorizationException(request);
            }
        }

        // This will create a new context if the ContextId is Guid.Empty
        return savedChatHistory ?? ChatHistory.CreateNew(request.Username);
    }

    private async Task<string> GenerateTitleAsync(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        var tempHistory = ChatHistory.CreateNew(request.Username);

        ChatMessage titleUserMessage = ChatMessage.CreateUserMessage(request.Content, PromptKey.Title);

        tempHistory.AddMessage(titleUserMessage);

        ChatMessage titleAssistantMessage = await _chatCompletionRepository
            .GetChatCompletionAsync(tempHistory, cancellationToken);

        return titleAssistantMessage.Content;
    }
}
