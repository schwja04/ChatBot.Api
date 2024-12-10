using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Domain.ChatContextEntity;
using ChatBot.Api.Domain.Exceptions.ChatContextExceptions;
using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Commands.ProcessChatMessage;

internal class ProcessChatMessageCommandHandler(
    IChatCompletionRepository chatCompletionRepository,
    IChatContextRepository chatContextRepository) 
    : IRequestHandler<ProcessChatMessageCommand, ProcessChatMessageCommandResponse>
{
    private readonly IChatCompletionRepository _chatCompletionRepository = chatCompletionRepository;
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;

    public async Task<ProcessChatMessageCommandResponse> Handle(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        ChatContext chatContext = await GetChatHistoryAsync(request, cancellationToken);

        if (string.IsNullOrWhiteSpace(chatContext.Title))
        {
            string title = await GenerateTitleAsync(request, cancellationToken);
            chatContext.SetTitle(title);
        }

        ChatMessage userMessage = ChatMessage.CreateUserMessage(request.Content, request.PromptKey);
        chatContext.AddMessage(userMessage);

        // Send the chat history to openai and save the response to the chat history
        ChatMessage assistantMessage = await _chatCompletionRepository.GetChatCompletionAsync(chatContext, cancellationToken);
        chatContext.AddMessage(assistantMessage);

        await _chatContextRepository.SaveAsync(chatContext, cancellationToken);

        return new ProcessChatMessageCommandResponse
        {
            ContextId = chatContext.ContextId,
            ChatMessage = assistantMessage
        };
    }

    private async Task<ChatContext> GetChatHistoryAsync(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        ChatContext? savedChatHistory = null;

        // Check if the context already exists
        if (request.ContextId != Guid.Empty)
        {
            savedChatHistory = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

            if (savedChatHistory is null)
            {
                throw new ChatContextNotFoundException(request.ContextId, request.Username);
            }

            if (!string.Equals(savedChatHistory.Username, request.Username, StringComparison.OrdinalIgnoreCase))
            {
                throw new ChatContextAuthorizationException(request.ContextId, request.Username);
            }
        }

        // This will create a new context if the ContextId is Guid.Empty
        return savedChatHistory ?? ChatContext.CreateNew(request.Username);
    }

    private async Task<string> GenerateTitleAsync(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        var tempHistory = ChatContext.CreateNew(request.Username);

        ChatMessage titleUserMessage = ChatMessage.CreateUserMessage(request.Content, PromptKey.Title);

        tempHistory.AddMessage(titleUserMessage);

        ChatMessage titleAssistantMessage = await _chatCompletionRepository
            .GetChatCompletionAsync(tempHistory, cancellationToken);

        return titleAssistantMessage.Content;
    }
}
