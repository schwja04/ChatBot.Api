using AutoFixture;
using ChatBot.Application;
using ChatBot.Application.Queries.GetManyPrompts;
using ChatBot.Domain.PromptEntity;
using FluentAssertions;
using NSubstitute;

namespace ChatBot.UnitTests.Application.Handlers.QueryHandlers;

public class GetManyPromptQueryHandlerShould
{
	private readonly IPromptRepository _promptRepository;

	private readonly GetManyPromptsQueryHandler _sut;

	private readonly IFixture _fixture;

	public GetManyPromptQueryHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();

		_sut = new GetManyPromptsQueryHandler(_promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task Handle_ShouldReturnOnlyUserPrompts_WhenIncludeSystemPromptsIsFalse()
	{
		// Arrange
		var query = _fixture
			.Build<GetManyPromptsQuery>()
			.With(x => x.IncludeSystemPrompts, false)
			.Create();
		var cancellationToken = CancellationToken.None;
		
		var userPrompts = new List<Prompt>()
		{
			Prompt.CreateNew(
				_fixture.Create<string>(),
				_fixture.Create<string>(),
				query.Username)
		}.AsReadOnly();

		_promptRepository
			.GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(userPrompts);

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Count.Should().Be(userPrompts.Count);
		result.All(p => string.Equals(p.Owner, query.Username)).Should().BeTrue();
	}
	
	[Fact]
	public async Task Handle_ShouldReturnUserAndSystemPrompts_WhenIncludeSystemPromptsIsTrue()
	{
		// Arrange
		var query = _fixture
			.Build<GetManyPromptsQuery>()
			.With(x => x.IncludeSystemPrompts, true)
			.Create();
		var cancellationToken = CancellationToken.None;

		var userPrompts = new List<Prompt>()
		{
			Prompt.CreateNew(
				_fixture.Create<string>(),
				_fixture.Create<string>(),
				query.Username)
		}.AsReadOnly();
		
		var systemPrompts = new List<Prompt>()
		{
			Prompt.CreateNew(
				_fixture.Create<string>(),
				_fixture.Create<string>(),
				Constants.SystemUser)
		}.AsReadOnly();

		_promptRepository
			.GetManyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(userPrompts, systemPrompts);

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Count.Should().Be(userPrompts.Count + systemPrompts.Count);
		result.Count(p => string.Equals(p.Owner, query.Username)).Should().Be(userPrompts.Count);
		result.Count(p => string.Equals(p.Owner, Constants.SystemUser)).Should().Be(systemPrompts.Count);
	}
}
