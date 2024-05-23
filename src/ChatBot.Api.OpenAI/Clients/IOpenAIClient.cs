using ChatBot.Api.OpenAI.Models;

namespace ChatBot.Api.OpenAI.Clients;

public interface IOpenAIClient
{
    Task<CreateChatCompletionResponse> CreateChatCompletionAsync(CreateChatCompletionRequest request, CancellationToken cancellationToken = default);
}
