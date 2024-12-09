using AutoFixture;
using ChatBot.Api.Application.Commands.DeletePrompt;
using ChatBot.Api.Domain.Exceptions.PromptExceptions;
using ChatBot.Api.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.Api.UnitTests.Application.Handlers.CommandHandlers;

public class DeletePromptCommandHandlerShould
{
	private readonly IPromptRepository _promptRepository;
	private readonly DeletePromptCommandHandler _sut;
	private readonly IFixture _fixture;

    public DeletePromptCommandHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		_sut = new DeletePromptCommandHandler(_promptRepository);

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
	public async Task Handle_ShouldDelete_WhenPromptExistsAndOwnedByRequestor()
	{
		// Arrange
		var prompt = _fixture.Create<Prompt>();
		var cmd = new DeletePromptCommand
		{
			Username = prompt.Owner,
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
			Username = "DifferentUser",
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

