using System.Collections.ObjectModel;
using ChatBot.Application.Abstractions.Repositories;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Builders;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using Common.OpenAI.Clients;
using Common.OpenAI.Models;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion;

internal class ChatCompletionRepository(IOpenAIClient openAIClient, IPromptMessageMapper promptMapper)
    : IChatCompletionRepository
{
    private readonly IOpenAIClient _openAIClient = openAIClient;
    private readonly IPromptMessageMapper _promptMapper = promptMapper;

    // TODO: ADD ILogger<ChatCompletionRepository>
    // This will allow me to log information about the chat completion

    // TODO: ADD IOptionsMonitor<OpenAIOptions>
    // This will allow me to dynamically change the model on the fly
    
    public async Task<ChatMessage> GetChatCompletionAsync(ChatContext chatContext, CancellationToken cancellationToken)
    {
        // Convert ChatHistory to CreateChatCompletionRequest
        var messages = await MapMessagesAsync(chatContext, cancellationToken);

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

    private async Task<ReadOnlyCollection<ChatCompletionRequestMessage>> MapMessagesAsync(ChatContext chatContext, CancellationToken cancellationToken)
    {
        List<ChatCompletionRequestMessage> reqMessages = new(chatContext.Messages.Count);

        ChatMessage currentMessage;
        // Want to loop over all but last
        for (int i = 0; i < chatContext.Messages.Count - 1; i++)
        {
            currentMessage = chatContext.Messages[i];
            reqMessages.Add(new ChatCompletionRequestMessage
            {
                Role = Enum.GetName(currentMessage.Role)!.ToLowerInvariant(),
                Content = currentMessage.Content
            });
        }

        currentMessage = chatContext.Messages[^1];
        reqMessages.Add(new ChatCompletionRequestMessage
        {
            Role = Enum.GetName(currentMessage.Role)!.ToLowerInvariant(),
            Content = await _promptMapper.BuildMessageContentAsync(currentMessage, chatContext.Username, cancellationToken)
        });

        return reqMessages.AsReadOnly();
    }
}

