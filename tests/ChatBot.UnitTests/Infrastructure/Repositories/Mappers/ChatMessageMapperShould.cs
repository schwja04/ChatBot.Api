using AutoFixture;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using FluentAssertions;
using Microsoft.Extensions.AI;
using NSubstitute;
using ChatMessage = ChatBot.Domain.ChatContextEntity.ChatMessage;

namespace ChatBot.UnitTests.Infrastructure.Repositories.Mappers;

public class ChatMessageMapperShould
{
    private readonly IPromptMessageMapper _promptMapper;
    private readonly Fixture _fixture;
    
    private readonly ChatMessageMapper _sut;
    
    public ChatMessageMapperShould()
    {
        _promptMapper = Substitute.For<IPromptMessageMapper>();
        _fixture = new Fixture();
        
        _sut = new ChatMessageMapper(_promptMapper);
    }

    [Fact]
    public async Task ToLLMChatMessagesAsync_ShouldMapChatContextMessagesToLLMMessages()
    {
        // Arrange
        var chatMessages = new[]
        {
            ChatMessage.CreateUserMessage("Hello, how are you?", "None")
        };

        var chatContext = ChatContext.CreateExisting(
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            chatMessages,
            _fixture.Create<DateTimeOffset>(),
            _fixture.Create<DateTimeOffset>());

        _promptMapper
            .BuildMessageContentAsync(
                Arg.Any<ChatMessage>(), 
                Arg.Any<string>(), 
                Arg.Any<CancellationToken>())
            .Returns(chatMessages[0].Content);
        
        // Act
        var result = await _sut.ToLLMChatMessagesAsync(chatContext, CancellationToken.None);
        
        // Assert
        result.Count.Should().Be(1);
        result[0].Role.Should().Be(ChatRole.User);
        result[0].Text.Should().Be(chatMessages[0].Content);
    }
}