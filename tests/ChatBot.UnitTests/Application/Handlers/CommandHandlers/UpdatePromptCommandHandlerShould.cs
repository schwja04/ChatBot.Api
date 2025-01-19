using AutoFixture;
using ChatBot.Application.Commands.UpdatePrompt;
using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.UnitTests.Application.Handlers.CommandHandlers;

public class UpdatePromptCommandHandlerShould
{
	private readonly IPromptRepository _promptRepository;

	private readonly UpdatePromptCommandHandler _sut;

	private readonly IFixture _fixture;

	public UpdatePromptCommandHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		_sut = new UpdatePromptCommandHandler(_promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task Handle_ShouldSavePrompt()
	{
		// Arrange
		var cmd = _fixture.Create<UpdatePromptCommand>();
		var cancellationToken = CancellationToken.None;

		var prompt = Prompt.CreateExisting(
			cmd.PromptId, 
			key: _fixture.Create<string>(), 
			value: _fixture.Create<string>(), 
			cmd.Owner);
		
		_promptRepository
			.GetAsync(cmd.PromptId, cancellationToken)
			.Returns(prompt);
		
		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().NotThrowAsync();

		await _promptRepository
			.Received(1)
			.UpdateAsync(
				Arg.Is<Prompt>(x =>
					x.PromptId == cmd.PromptId
					&& x.Key == cmd.Key
					&& x.Value == cmd.Value
					&& x.Owner == cmd.Owner),
				Arg.Any<CancellationToken>());
	}
	
	[Fact]
	public async Task Handle_ShouldThrowException_WhenPromptNotFound()
	{
		// Arrange
		var cmd = _fixture.Create<UpdatePromptCommand>();
		var cancellationToken = CancellationToken.None;
		
		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().ThrowAsync<PromptNotFoundException>();
	}
	
	[Fact]
	public async Task Handle_ShouldThrowException_WhenPromptOwnerIsDifferent()
	{
		// Arrange
		var cmd = _fixture.Create<UpdatePromptCommand>();
		var cancellationToken = CancellationToken.None;

		var prompt = Prompt.CreateExisting(
			cmd.PromptId, 
			key: _fixture.Create<string>(), 
			value: _fixture.Create<string>(), 
			_fixture.Create<string>());
		
		_promptRepository
			.GetAsync(cmd.PromptId, cancellationToken)
			.Returns(prompt);
		
		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().ThrowAsync<PromptAuthorizationException>();
	}
	
	[Fact]
	public async Task Handle_ShouldThrowException_WhenPromptKeyAlreadyExists()
	{
		// Arrange
		var cmd = _fixture.Create<UpdatePromptCommand>();
		var cancellationToken = CancellationToken.None;

		var promptToUpdate = Prompt.CreateExisting(
			cmd.PromptId, 
			key: _fixture.Create<string>(), 
			value: _fixture.Create<string>(), 
			cmd.Owner);
		
		var existingPrompt = Prompt.CreateExisting(
			Guid.NewGuid(), 
			key: cmd.Key, 
			value: _fixture.Create<string>(), 
			cmd.Owner);
		
		_promptRepository
			.GetAsync(cmd.PromptId, cancellationToken)
			.Returns(promptToUpdate);
		
		_promptRepository
			.GetAsync(cmd.Owner, cmd.Key, cancellationToken)
			.Returns(existingPrompt);
		
		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().ThrowAsync<PromptDuplicateKeyException>();
	}
}

