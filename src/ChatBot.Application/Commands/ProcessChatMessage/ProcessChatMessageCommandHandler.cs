using ChatBot.Application.Abstractions.Repositories;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.Exceptions.ChatContextExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Commands.ProcessChatMessage;

internal class ProcessChatMessageCommandHandler(
    ILogger<ProcessChatMessageCommandHandler> logger,
    IChatCompletionRepository chatCompletionRepository,
    IChatContextRepository chatContextRepository) 
    : IRequestHandler<ProcessChatMessageCommand, ProcessChatMessageCommandResponse>
{
    private readonly IChatCompletionRepository _chatCompletionRepository = chatCompletionRepository;
    private readonly IChatContextRepository _chatContextRepository = chatContextRepository;
    private readonly ILogger _logger = logger;

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
        _logger.LogInformation(
            "Sending chatContext ({ContextId}) to openai for user {Username}",
            chatContext.ContextId,
            request.UserId);
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
        if (request.ContextId == Guid.Empty)
        {
            var newChatHistory = ChatContext.CreateNew(request.UserId);
            _logger.LogInformation(
                "Creating new chatContext ({ContextId}) for user {Username}",
                newChatHistory.ContextId,
                request.UserId);
            return newChatHistory;
        }

        _logger.LogInformation(
            "Retrieving chatContext ({ContextId}) for user {Username}",
            request.ContextId,
            request.UserId);
        var savedChatHistory = await _chatContextRepository.GetAsync(request.ContextId, cancellationToken);

        if (savedChatHistory is null)
        {
            _logger.LogError(
                "ChatContext ({ContextId}) not found for user {Username}",
                request.ContextId,
                request.UserId);
            throw new ChatContextNotFoundException(request.ContextId, request.UserId);
        }

        if (savedChatHistory.UserId != request.UserId)
        {
            _logger.LogWarning(
                "User ({UserId}) attempted to access chatContext ({ContextId}) that belongs to {Owner}",
                request.UserId,
                request.ContextId,
                savedChatHistory.UserId);
            throw new ChatContextAuthorizationException(request.ContextId, request.UserId);
        }
        
        _logger.LogInformation(
            "Retrieved chatContext ({ContextId}) for user {UserId}",
            request.ContextId,
            request.UserId);
        return savedChatHistory;
    }

    private async Task<string> GenerateTitleAsync(ProcessChatMessageCommand request, CancellationToken cancellationToken)
    {
        var tempHistory = ChatContext.CreateNew(request.UserId);

        ChatMessage titleUserMessage = ChatMessage.CreateUserMessage(request.Content, PromptKey.Title);

        tempHistory.AddMessage(titleUserMessage);

        _logger.LogInformation(
            "Generating a title for new chat context with openai for user {Username}",
            request.UserId);
        ChatMessage titleAssistantMessage = await _chatCompletionRepository
            .GetChatCompletionAsync(tempHistory, cancellationToken);

        return titleAssistantMessage.Content;
    }
}
