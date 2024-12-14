using Common.OpenAI.Clients;
using Common.OpenAI.Models;
using Newtonsoft.Json;
using NSubstitute;

namespace ChatBot.Api.IntegrationTests.WebApplicationFactories.MockImplementations;

public class SubstituteOpenAIClient : IOpenAIClient
{
    private readonly IOpenAIClient _openAIClient;
    
    public SubstituteOpenAIClient()
    {
        _openAIClient = Substitute.For<IOpenAIClient>();
        _openAIClient.CreateChatCompletionAsync(
                Arg.Any<CreateChatCompletionRequest>(), 
                Arg.Any<CancellationToken>())
            .Returns(CreateChatCompletionResponseFromJson(CreateChatCompletionResponseJson));
    }
    public async Task<CreateChatCompletionResponse> CreateChatCompletionAsync(
        CreateChatCompletionRequest request, 
        CancellationToken cancellationToken = default)
    {
        return await _openAIClient.CreateChatCompletionAsync(request, cancellationToken);
    }
    
    private static CreateChatCompletionResponse CreateChatCompletionResponseFromJson(string json)
    {
        return JsonConvert.DeserializeObject<CreateChatCompletionResponse>(json)!;
    }
    
    private const string CreateChatCompletionResponseJson = $$"""
                                                              {
                                                                "id": "chatcmpl-292",
                                                                "object": "chat.completion",
                                                                "created": 1733973252,
                                                                "model": "llama3",
                                                                "system_fingerprint": "fp_ollama",
                                                                "choices": [
                                                                    {
                                                                        "index": 0,
                                                                        "message": {
                                                                            "role": "assistant",
                                                                            "content": "Hello there! It's great to see you! Welcome to our little conversation corner. I'm happy to chat with you about anything that interests you. What would you like to talk about?"
                                                                        },
                                                                        "finish_reason": "stop"
                                                                    }
                                                                ],
                                                                "usage": {
                                                                    "prompt_tokens": 14,
                                                                    "completion_tokens": 39,
                                                                    "total_tokens": 53
                                                                }
                                                              }
                                                              """;
}