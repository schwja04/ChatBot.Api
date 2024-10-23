using AutoFixture;
using ChatBot.Api.Application.CommandHandlers;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.Api.UnitTests.Application.Handlers.CommandHandlers;

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
			.SaveAsync(result, Arg.Any<CancellationToken>());
	}
}

