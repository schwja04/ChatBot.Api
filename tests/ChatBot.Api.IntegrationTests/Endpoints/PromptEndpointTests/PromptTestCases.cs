using System.Net;
using AutoFixture;
using ChatBot.Api.Contracts;
using ChatBot.Api.IntegrationTests.Endpoints.TestHelpers;
using FluentAssertions;

namespace ChatBot.Api.IntegrationTests.Endpoints.PromptEndpointTests;

public static class PromptTestCases
{
    public static async Task RunAsync(HttpClient client, Fixture fixture)
    {
        #region GetManyPromptsBeforeCreate
        var getManyPromptsHttpResponse = await client.GetAsync("/api/prompts");
        
        getManyPromptsHttpResponse.EnsureSuccessStatusCode();

        var getManyPromptsResponse = await getManyPromptsHttpResponse.Content.DeserializeAsync<GetManyPromptsResponse>();
        getManyPromptsResponse!.Prompts.Should().BeEmpty();
        #endregion
        
        #region CreatePrompt
        var createPromptRequest = fixture.Create<CreatePromptRequest>();
        var createPromptHttpResponse = await client.PostAsJsonAsync("/api/prompts", createPromptRequest);
        
        createPromptHttpResponse.EnsureSuccessStatusCode();
        var createPromptResponse = await createPromptHttpResponse.Content.DeserializeAsync<CreatePromptResponse>();
        
        createPromptHttpResponse.Headers.Location.Should().NotBeNull();
        createPromptResponse!.Key.Should().Be(createPromptRequest.Key);
        createPromptResponse.Value.Should().Be(createPromptRequest.Value);
        createPromptResponse.Owner.Should().NotBeNullOrEmpty();

        Guid promptId = createPromptResponse.PromptId;
        #endregion

        #region GetPromptAfterCreate
        var getPromptHttpResponse = await client.GetAsync($"/api/prompts/{promptId}");
        
        getPromptHttpResponse.EnsureSuccessStatusCode();
        var getPromptResponse = await getPromptHttpResponse.Content.DeserializeAsync<GetPromptResponse>();

        getPromptResponse.Should().BeEquivalentTo(createPromptResponse);
        #endregion
        
        #region GetManyPromptsAfterCreate
        getManyPromptsHttpResponse = await client.GetAsync("/api/prompts");
        
        getManyPromptsHttpResponse.EnsureSuccessStatusCode();

        getManyPromptsResponse = await getManyPromptsHttpResponse.Content.DeserializeAsync<GetManyPromptsResponse>();
        getManyPromptsResponse!.Prompts.Should().HaveCount(1);
        
        getManyPromptsResponse.Prompts.Single().Should().BeEquivalentTo(createPromptResponse);
        #endregion
        
        #region UpdatePrompt
        var updatePromptRequest = fixture.Create<UpdatePromptRequest>();
        var updatePromptHttpResponse = await client.PutAsJsonAsync($"/api/prompts/{promptId}", updatePromptRequest);
        
        updatePromptHttpResponse.EnsureSuccessStatusCode();
        #endregion

        #region GetPromptAfterUpdate
        getPromptHttpResponse = await client.GetAsync($"/api/prompts/{promptId}");

        getPromptHttpResponse.EnsureSuccessStatusCode();
        getPromptResponse = await getPromptHttpResponse.Content.DeserializeAsync<GetPromptResponse>();
        
        getPromptResponse!.Key.Should().Be(updatePromptRequest.Key);
        getPromptResponse.Value.Should().Be(updatePromptRequest.Value);
        #endregion
        
        #region DeletePrompt
        var deletePromptHttpResponse = await client.DeleteAsync($"/api/prompts/{promptId}");
        
        deletePromptHttpResponse.EnsureSuccessStatusCode();
        #endregion
        
        #region GetPromptAfterDelete
        getPromptHttpResponse = await client.GetAsync($"/api/prompts/{promptId}");
         
        getPromptHttpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        #endregion
    }
}