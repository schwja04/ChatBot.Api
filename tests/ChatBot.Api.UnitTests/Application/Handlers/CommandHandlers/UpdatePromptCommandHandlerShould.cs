using AutoFixture;
using ChatBot.Api.Application.Commands.UpdatePrompt;
using ChatBot.Api.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.Api.UnitTests.Application.Handlers.CommandHandlers;

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

		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().NotThrowAsync();

		await _promptRepository
			.Received(1)
			.SaveAsync(
				Arg.Is<Prompt>(x =>
					x.PromptId == cmd.PromptId
					&& x.Key == cmd.Key
					&& x.Value == cmd.Value
					&& x.Owner == cmd.Owner),
				Arg.Any<CancellationToken>());
	}
}

