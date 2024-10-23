using System.Collections.ObjectModel;
using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Domain.ChatHistoryEntity;
using ChatBot.Api.Infrastructure.Repositories.Builders;
using ChatBot.Api.Infrastructure.Repositories.Mappers;
using Common.OpenAI.Clients;
using Common.OpenAI.Models;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class ChatCompletionRepository : IChatCompletionRepository
{
    private readonly IOpenAIClient _openAIClient;
    private readonly IPromptMessageMapper _promptMapper;

    // TODO: ADD ILogger<ChatCompletionRepository>
    // This will allow me to log information about the chat completion

    // TODO: ADD IOptionsMonitor<OpenAIOptions>
    // This will allow me to dynamically change the model on the fly

    public ChatCompletionRepository(IOpenAIClient openAIClient, IPromptMessageMapper promptMapper)
    {
        _openAIClient = openAIClient;
        _promptMapper = promptMapper;
    }

    public async Task<ChatMessage> GetChatCompletionAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        // Convert ChatHistory to CreateChatCompletionRequest
        var messages = await MapMessagesAsync(chatHistory, cancellationToken);

        CreateChatCompletionRequest request = new CreateChatCompletionRequestBuilder()
            .UseAllDefaults()
            .WithModel("llama3")
            .WithMessages(messages)
            .Build();

        // Call OpenAI API
        CreateChatCompletionResponse response = await _openAIClient.CreateChatCompletionAsync(request, cancellationToken);

        // Convert CreateChatCompletionResponse to ChatMessage
        var choice = response.Choices.First();
        ChatMessage chatMessage = ChatMessage.CreateAssistantMessage(choice.Message.Content);

        return chatMessage;
    }

    private async Task<ReadOnlyCollection<ChatCompletionRequestMessage>> MapMessagesAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        List<ChatCompletionRequestMessage> reqMessages = new(chatHistory.ChatMessages.Count);

        ChatMessage currentMessage;
        // Want to loop over all but last
        for (int i = 0; i < chatHistory.ChatMessages.Count - 1; i++)
        {
            currentMessage = chatHistory.ChatMessages[i];
            reqMessages.Add(new ChatCompletionRequestMessage
            {
                Role = Enum.GetName(currentMessage.Role)!.ToLowerInvariant(),
                Content = currentMessage.Content
            });
        }

        currentMessage = chatHistory.ChatMessages[^1];
        reqMessages.Add(new ChatCompletionRequestMessage
        {
            Role = Enum.GetName(currentMessage.Role)!.ToLowerInvariant(),
            Content = await _promptMapper.BuildMessageContentAsync(currentMessage, chatHistory.Username, cancellationToken)
        });

        return reqMessages.AsReadOnly();
    }
}

