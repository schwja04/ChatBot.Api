using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Infrastructure.Repositories.Builders;
using ChatBot.Api.OpenAI.Clients;
using ChatBot.Api.OpenAI.Models;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class ChatCompletionRepository : IChatCompletionRepository
{
    private readonly IOpenAIClient _openAIClient;

    // TODO: ADD ILogger<ChatCompletionRepository>
    // This will allow me to log information about the chat completion

    // TODO: ADD IOptionsMonitor<OpenAIOptions>
    // This will allow me to dynamically change the model on the fly

    public ChatCompletionRepository(IOpenAIClient openAIClient)
    {
        _openAIClient = openAIClient;
    }

    public async Task<ChatMessage> GetChatCompletionAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        // Convert ChatHistory to CreateChatCompletionRequest
        ChatCompletionRequestMessage[] messages = chatHistory.ChatMessages.Select(m => new ChatCompletionRequestMessage
        {
            Role = Enum.GetName(m.Role)!.ToLowerInvariant(),
            Content = m.Content
        }).ToArray();

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
}

