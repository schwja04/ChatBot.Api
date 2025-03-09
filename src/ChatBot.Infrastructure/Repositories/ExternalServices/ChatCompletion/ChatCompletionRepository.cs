using ChatBot.Application.Abstractions.Repositories;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ChatBotChatMessage = ChatBot.Domain.ChatContextEntity.ChatMessage;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion;

internal class ChatCompletionRepository : IChatCompletionRepository
{
    private readonly ILogger _logger;
    private readonly IChatClient _chatClient;
    private readonly IChatMessageMapper _chatMessageMapper;
    
    private ChatCompletionOptions _options;
    
    public ChatCompletionRepository(
        ILogger<ChatCompletionRepository> logger,
        IOptionsMonitor<ChatCompletionOptions> chatCompletionOptions,
        IChatClient chatClient, 
        IChatMessageMapper chatMessageMapper)
    {
        _chatClient = chatClient;
        _logger = logger;
        _chatMessageMapper = chatMessageMapper;
        _options = chatCompletionOptions.CurrentValue;
        
        chatCompletionOptions.OnChange(OnOptionsChange);
    }

    public async Task<ChatBotChatMessage> GetChatCompletionAsync(ChatContext chatContext, CancellationToken cancellationToken)
    {
        var  messages = await _chatMessageMapper
            .ToLLMChatMessagesAsync(chatContext, cancellationToken);
        
        _logger.LogInformation("Sending chatContext ({ContextId}) to llm for user {Username}, using model {Model}",
            chatContext.ContextId,
            chatContext.Username,
            _options.DefaultModel);
        
        var response = await _chatClient.GetResponseAsync(messages, options: new ChatOptions
        {
            ModelId = _options.DefaultModel,
            FrequencyPenalty = _options.DefaultFrequencyPenalty,
            PresencePenalty = _options.DefaultPresencePenalty,
            Temperature = _options.DefaultTemperature,
            TopP = _options.DefaultTopP,
            MaxOutputTokens = _options.DefaultMaxTokens,
            
        }, cancellationToken);
        
        _logger.LogInformation("Received response from llm for chatContext ({ContextId}) for user {Username}. Prompt tokens used: {PromptTokens}. Completion tokens used: {ResponseTokens}. Total tokens used: {TotalTokens}",
            chatContext.ContextId,
            chatContext.Username,
            response.Usage!.InputTokenCount,
            response.Usage.OutputTokenCount,
            response.Usage.TotalTokenCount);
        
        return ChatBotChatMessage.CreateAssistantMessage(response.Message.Text!);
    }
    
    private void OnOptionsChange(ChatCompletionOptions options)
    {
        _options = options;
    }
}