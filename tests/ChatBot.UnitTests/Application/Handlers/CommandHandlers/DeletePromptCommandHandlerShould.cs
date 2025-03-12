using AutoFixture;
using ChatBot.Application.Commands.DeletePrompt;
using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChatBot.UnitTests.Application.Handlers.CommandHandlers;

public class DeletePromptCommandHandlerShould
{
	private readonly IPromptRepository _promptRepository;
	private readonly DeletePromptCommandHandler _sut;
	private readonly IFixture _fixture;

    public DeletePromptCommandHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		var logger = NullLogger<DeletePromptCommandHandler>.Instance;
		_sut = new DeletePromptCommandHandler(logger, _promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task Handle_ShouldThrow_WhenPromptIsNotFound()
	{
		// Arrange
		var cmd = _fixture.Create<DeletePromptCommand>();
		var cancellationToken = CancellationToken.None;

		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().ThrowAsync<PromptNotFoundException>();
		await _promptRepository
			.Received(0)
			.DeleteAsync(Arg.Any<Prompt>(), Arg.Any<CancellationToken>());
	}
	
	[Fact]
	public async Task Handle_ShouldDelete_WhenPromptExistsAndOwnedByRequester()
	{
		// Arrange
		var prompt = _fixture.Create<Prompt>();
		var cmd = new DeletePromptCommand
		{
			UserId = prompt.OwnerId,
			PromptId = prompt.PromptId
		};
		var cancellationToken = CancellationToken.None;

		_promptRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(prompt);
		
		// Act
		await _sut.Handle(cmd, cancellationToken);

		// Assert
		await _promptRepository
			.Received(1)
			.DeleteAsync(Arg.Any<Prompt>(), Arg.Any<CancellationToken>());
	}
	
	[Fact]
	public async Task Handle_ShouldThrow_WhenPromptOwnerIsDifferent()
	{
		// Arrange
		var prompt = _fixture.Create<Prompt>();
		var cmd = new DeletePromptCommand
		{
			UserId = Guid.NewGuid(),
			PromptId = prompt.PromptId
		};
		var cancellationToken = CancellationToken.None;

		_promptRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(prompt);
		
		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().ThrowAsync<PromptAuthorizationException>();
	}
}

