using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Application.Models.Commands;
using ChatBot.Api.Application.Models.Exceptions;

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
                throw new ChatHistoryNotFoundException(request.ContextId);
            }

            if (!string.Equals(chatHistory.Username, request.Username, StringComparison.OrdinalIgnoreCase))
            {
                throw new ChatHistoryAuthorizationException(request);
            }
        }

        // This will create a new context if the ContextId is Guid.Empty
        chatHistory ??= ChatHistory.CreateNew(request.Username);

        if (string.IsNullOrWhiteSpace(chatHistory.Title))
        {
            var tempHistory = ChatHistory.CreateNew(request.Username);

            ChatMessage titleUserMessage = ChatMessage.CreateUserMessage($"""
                Create a title for our conversation based on the following message. Ensure to keep the title to no more than 5 words. Do not include special characters.

                ### Starting Context Here ###
                {request.Content}
                """);

            tempHistory.AddMessage(titleUserMessage);

            ChatMessage titleAssistantMessage = await _chatCompletionRepository
                .GetChatCompletionAsync(tempHistory, cancellationToken);

            chatHistory.SetTitle(titleAssistantMessage.Content);
        }

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

