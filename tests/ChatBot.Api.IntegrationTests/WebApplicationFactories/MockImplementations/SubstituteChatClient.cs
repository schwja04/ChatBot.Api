using Microsoft.Extensions.AI;
using NSubstitute;

namespace ChatBot.Api.IntegrationTests.WebApplicationFactories.MockImplementations;

public class SubstituteChatClient : IChatClient
{
    private static readonly ChatResponse ChatResponse = new(
        new ChatMessage(
            ChatRole.Assistant, 
            """
            Hello there! It's great to see you! 
            Welcome to our little conversation corner. 
            I'm happy to chat with you about anything that interests you. 
            What would you like to talk about?
            """))
    {
        CreatedAt = DateTimeOffset.UtcNow,
        ModelId = "llama3",
        Usage = new UsageDetails
        {
            InputTokenCount = 14,
            OutputTokenCount = 39,
            TotalTokenCount = 53
        },
        FinishReason = ChatFinishReason.Stop
    };
    
    private readonly IChatClient _openAIClient;
    
    public SubstituteChatClient()
    {
        _openAIClient = Substitute.For<IChatClient>();
        _openAIClient.GetResponseAsync(
                Arg.Any<IList<ChatMessage>>(),
                Arg.Any<ChatOptions?>(),
                Arg.Any<CancellationToken>())
            .Returns(ChatResponse);
    }
    
    public void Dispose()
    {
        _openAIClient.Dispose();
    }

    public async Task<ChatResponse> GetResponseAsync(
        IList<ChatMessage> chatMessages, 
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await _openAIClient.GetResponseAsync(chatMessages, options, cancellationToken);
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IList<ChatMessage> chatMessages, 
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        throw new NotImplementedException();
    }
}