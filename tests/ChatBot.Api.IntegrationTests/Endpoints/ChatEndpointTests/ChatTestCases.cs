using ChatBot.Api.Contracts;
using ChatBot.Api.IntegrationTests.Endpoints.TestHelpers;
using FluentAssertions;

namespace ChatBot.Api.IntegrationTests.Endpoints.ChatEndpointTests;

public static class ChatTestCases
{
    public static async Task RunAsync(HttpClient client)
    {
        Guid contextId = Guid.Empty;
        
        #region GetManyChatsBeforeCreate
        var getManyChatsMetadataHttpResponse = await client.GetAsync("/api/chats/metadata");
        
        getManyChatsMetadataHttpResponse.EnsureSuccessStatusCode();
        
        var getManyChatMetadataResponse = await getManyChatsMetadataHttpResponse
            .Content.DeserializeAsync<GetManyChatContextMetadataResponse>();
        getManyChatMetadataResponse!.ChatHistoryMetadatas.Should().BeEmpty();
        #endregion

        #region ProcessChatMessage
        var processChatMessageRequest = new ProcessChatMessageRequest
        {
            ContextId = contextId,
            Content = "Hello, World!",
            PromptKey = "None",
        };
        
        var processChatMessageHttpResponse = await client.PostAsJsonAsync("/api/chats", processChatMessageRequest);
        processChatMessageHttpResponse.EnsureSuccessStatusCode();
        
        var processChatMessageResponse = await processChatMessageHttpResponse
            .Content.DeserializeAsync<ProcessChatMessageResponse>();
        
        processChatMessageResponse!.ContextId.Should().NotBeEmpty();
        processChatMessageResponse.ChatMessage.Content.Should().NotBeNullOrEmpty();
        contextId = processChatMessageResponse.ContextId;
        #endregion
        
        #region GetChatAfterProcess
        var getChatHttpResponse = await client.GetAsync($"/api/chats/{contextId}");
        getChatHttpResponse.EnsureSuccessStatusCode();
        
        var getChatResponse = await getChatHttpResponse.Content.DeserializeAsync<GetChatContextResponse>();
        getChatResponse!.ChatMessages.Should().HaveCount(2);
        
        getChatResponse.ChatMessages.First().Content.Should().Be("Hello, World!");
        getChatResponse.Title.Should().NotBeNullOrEmpty();
        var title = getChatResponse.Title;
        #endregion

        #region UpdateChatTitle
        var newTitle = "New Title";
        var updateChatTitleRequest = new UpdateChatContextTitleRequest()
        {
            Title = newTitle
        };
        
        var updateChatTitleHttpResponse = await client.PutAsJsonAsync($"/api/chats/{contextId}:update-title", updateChatTitleRequest);
        updateChatTitleHttpResponse.EnsureSuccessStatusCode();
        #endregion
        
        #region GetChatAfterUpdate
        getChatHttpResponse = await client.GetAsync($"/api/chats/{contextId}");
        getChatHttpResponse.EnsureSuccessStatusCode();
        
        getChatResponse = await getChatHttpResponse.Content.DeserializeAsync<GetChatContextResponse>();
        getChatResponse!.Title.Should().Be(newTitle);
        title.Should().NotBe(newTitle);
        #endregion
        
        #region GetManyChatsAfterCreate
        getManyChatsMetadataHttpResponse = await client.GetAsync("/api/chats/metadata");
        getManyChatsMetadataHttpResponse.EnsureSuccessStatusCode();
        
        getManyChatMetadataResponse = await getManyChatsMetadataHttpResponse
            .Content.DeserializeAsync<GetManyChatContextMetadataResponse>();
        
        getManyChatMetadataResponse!.ChatHistoryMetadatas.Should().HaveCount(1);
        #endregion

        #region DeleteChat
        var deleteChatHttpResponse = await client.DeleteAsync($"/api/chats/{contextId}");
        deleteChatHttpResponse.EnsureSuccessStatusCode();
        #endregion
        
        #region GetManyChatsAfterDelete
        getManyChatsMetadataHttpResponse = await client.GetAsync("/api/chats/metadata");
        getManyChatsMetadataHttpResponse.EnsureSuccessStatusCode();
        
        getManyChatMetadataResponse = await getManyChatsMetadataHttpResponse
            .Content.DeserializeAsync<GetManyChatContextMetadataResponse>();
        
        getManyChatMetadataResponse!.ChatHistoryMetadatas.Should().BeEmpty();
        #endregion
    }
}