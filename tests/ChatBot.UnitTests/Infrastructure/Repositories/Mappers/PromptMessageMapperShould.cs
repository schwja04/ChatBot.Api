using AutoFixture;
using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.UnitTests.Infrastructure.Repositories.Mappers;

public class PromptMessageMapperShould
{
	private readonly IPromptRepository _promptRepository;

	private readonly PromptMessageMapper _sut;
	private readonly IFixture _fixture;

	public PromptMessageMapperShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		_sut = new PromptMessageMapper(_promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task BuildMessageContentAsync_ShouldReturnOriginalContent_WhenPromptKeyIsNone()
	{
		// Arrange
		string username = _fixture.Create<string>();
		ChatMessage chatMessage = ChatMessage.CreateUserMessage(
			_fixture.Create<string>(),
			"None");
		var cancellationToken = CancellationToken.None;

		// Act
		string result = await _sut.BuildMessageContentAsync(chatMessage, username, cancellationToken);

		// Assert
		result.Should().Be(chatMessage.Content);

		await _promptRepository
			.DidNotReceive()
			.GetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
	}

    [Fact]
    public async Task BuildMessageContentAsync_ShouldReturnOriginalContent_WhenPromptIsNotFoundForKey()
    {
        // Arrange
        string username = _fixture.Create<string>();
        ChatMessage chatMessage = ChatMessage.CreateUserMessage(
            _fixture.Create<string>(),
            _fixture.Create<string>());
        var cancellationToken = CancellationToken.None;

        // Act
        string result = await _sut.BuildMessageContentAsync(chatMessage, username, cancellationToken);

        // Assert
        result.Should().Be(chatMessage.Content);

        await _promptRepository
			.Received(1)
			.GetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BuildMessageContentAsync_ShouldReturnFormattedMessage()
    {
        // Arrange
        string username = _fixture.Create<string>();
        ChatMessage chatMessage = ChatMessage.CreateUserMessage(
            _fixture.Create<string>(),
            _fixture.Create<string>());
        var cancellationToken = CancellationToken.None;

        string templatedString = "Formatted: {0}";

        Prompt prompt = Prompt.CreateNew(
            chatMessage.PromptKey,
            templatedString,
            username);

        _promptRepository.GetAsync(username, prompt.Key, cancellationToken).Returns(prompt);

        // Act
        string result = await _sut.BuildMessageContentAsync(chatMessage, username, cancellationToken);

        // Assert
        result.Should().Be(string.Format(templatedString, chatMessage.Content));

        await _promptRepository
            .Received(1)
            .GetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}

