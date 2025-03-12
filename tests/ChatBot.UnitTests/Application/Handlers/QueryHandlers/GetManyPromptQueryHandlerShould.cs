using AutoFixture;
using ChatBot.Application;
using ChatBot.Application.Queries.GetManyPrompts;
using ChatBot.Domain.PromptEntity;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
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
		var logger = NullLogger<GetManyPromptsQueryHandler>.Instance;

		_sut = new GetManyPromptsQueryHandler(logger, _promptRepository);

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
				query.UserId)
		}.AsReadOnly();

		_promptRepository
			.GetManyAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(userPrompts);

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Count.Should().Be(userPrompts.Count);
		result.All(p => p.OwnerId == query.UserId).Should().BeTrue();
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
				query.UserId)
		}.AsReadOnly();
		
		var systemPrompts = new List<Prompt>()
		{
			Prompt.CreateNew(
				_fixture.Create<string>(),
				_fixture.Create<string>(),
				Constants.SystemUser)
		}.AsReadOnly();

		_promptRepository
			.GetManyAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(userPrompts, systemPrompts);

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Count.Should().Be(userPrompts.Count + systemPrompts.Count);
		result.Count(p => p.OwnerId == query.UserId).Should().Be(userPrompts.Count);
		result.Count(p => p.OwnerId == Constants.SystemUser).Should().Be(systemPrompts.Count);
	}
}
