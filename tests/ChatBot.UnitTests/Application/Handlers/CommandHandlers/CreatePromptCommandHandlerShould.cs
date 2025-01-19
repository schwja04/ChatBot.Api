using AutoFixture;
using ChatBot.Application.Commands.CreatePrompt;
using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.UnitTests.Application.Handlers.CommandHandlers;

public class CreatePromptCommandHandlerShould
{
	private readonly IPromptRepository _promptRepository;

	private readonly CreatePromptCommandHandler _sut;

	private readonly IFixture _fixture;

	public CreatePromptCommandHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		_sut = new CreatePromptCommandHandler(_promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task Handle_ShouldSavePrompt()
	{
		// Arrange
		var cmd = _fixture.Create<CreatePromptCommand>();
		var cancellationToken = CancellationToken.None;

		// Act
		var result = await _sut.Handle(cmd, cancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeEquivalentTo(cmd, opts => opts.ExcludingMissingMembers());


		await _promptRepository
			.Received(1)
			.CreateAsync(result, Arg.Any<CancellationToken>());
	}
	
	[Fact]
	public async Task Handle_ShouldThrowException_WhenPromptAlreadyExistsWithSameKey()
	{
		// Arrange
		var cmd = _fixture.Create<CreatePromptCommand>();
		var prompt = Prompt.CreateNew(cmd.Key, _fixture.Create<string>(), cmd.Owner);
		var cancellationToken = CancellationToken.None;

		_promptRepository
			.GetAsync(cmd.Owner, cmd.Key, cancellationToken)
			.Returns(prompt);

		// Act
		Func<Task> act = async () => await _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().ThrowAsync<PromptDuplicateKeyException>();
	}
}

