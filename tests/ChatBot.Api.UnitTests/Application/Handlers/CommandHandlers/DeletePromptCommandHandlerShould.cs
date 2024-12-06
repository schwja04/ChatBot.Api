using AutoFixture;
using ChatBot.Api.Application.Commands.DeletePrompt;
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
	public async Task Handle_ShouldNotThrow()
	{
		// Arrange
		var cmd = _fixture.Create<DeletePromptCommand>();
		var cancellationToken = CancellationToken.None;

		// Act
		var act = () => _sut.Handle(cmd, cancellationToken);

		// Assert
		await act.Should().NotThrowAsync();
		await _promptRepository
			.Received(1)
			.DeleteAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
	}
}

