using AutoFixture;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Application.QueryHandlers;
using ChatBot.Api.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.Api.UnitTests.Application.Handlers.QueryHandlers;

public class GetPromptsQueryHandlerShould
{
	private readonly IPromptRepository _promptRepository;

	private readonly GetPromptsQueryHandler _sut;

	private readonly IFixture _fixture;

	public GetPromptsQueryHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		_sut = new GetPromptsQueryHandler(_promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task Handle_ShouldReturn_Prompts()
	{
		// Arrange
		var query = _fixture.Create<GetPromptsQuery>();
		var cancellationToken = CancellationToken.None;

		_promptRepository
			.GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(Array.Empty<Prompt>().AsReadOnly());

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Count.Should().Be(0);
	}
}
