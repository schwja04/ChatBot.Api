using Common.OpenAI.Models;

namespace Common.OpenAI.Clients;

public interface IOpenAIClient
{
    Task<CreateChatCompletionResponse> CreateChatCompletionAsync(CreateChatCompletionRequest request, CancellationToken cancellationToken = default);
}
