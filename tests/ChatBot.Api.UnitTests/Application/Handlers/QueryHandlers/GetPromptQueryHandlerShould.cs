using AutoFixture;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Application.QueryHandlers;
using ChatBot.Api.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.Api.UnitTests.Application.Handlers.QueryHandlers;

public class GetPromptQueryHandlerShould
{
	private readonly IPromptRepository _promptRepository;

	private readonly GetPromptQueryHandler _sut;

	private readonly IFixture _fixture;

	public GetPromptQueryHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		_sut = new GetPromptQueryHandler(_promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task Handle_WithPromptId_ShouldReturn_Prompt()
	{
		// Arrange
		var query = _fixture.Create<GetPromptQuery>();
		var cancellationToken = CancellationToken.None;

		var prompt = Prompt.CreateExisting(
			query.PromptId!.Value,
			_fixture.Create<string>(),
			_fixture.Create<string>(),
			query.Username);

		_promptRepository
			.GetAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(prompt);

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().Be(prompt);

		await _promptRepository
			.DidNotReceive()
			.GetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
	}

    [Fact]
    public async Task Handle_WithPromptKey_ShouldReturn_Prompt()
    {
		// Arrange
		var query = new GetPromptQuery(
			_fixture.Create<string>(),
			_fixture.Create<string>());
        var cancellationToken = CancellationToken.None;

        var prompt = Prompt.CreateExisting(
            _fixture.Create<Guid>(),
            query.PromptKey!,
            _fixture.Create<string>(),
            query.Username);

        _promptRepository
            .GetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(prompt);

        // Act
        var result = await _sut.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(prompt);

        await _promptRepository
            .DidNotReceive()
            .GetAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}

