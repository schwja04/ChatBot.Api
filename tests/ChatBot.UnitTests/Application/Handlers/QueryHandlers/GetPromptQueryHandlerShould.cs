﻿using AutoFixture;
using ChatBot.Application;
using ChatBot.Application.Queries.GetPrompt;
using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChatBot.UnitTests.Application.Handlers.QueryHandlers;

public class GetPromptQueryHandlerShould
{
	private readonly IPromptRepository _promptRepository;

	private readonly GetPromptQueryHandler _sut;

	private readonly IFixture _fixture;

	public GetPromptQueryHandlerShould()
	{
		_promptRepository = Substitute.For<IPromptRepository>();
		var logger = NullLogger<GetPromptQueryHandler>.Instance;
		
		_sut = new GetPromptQueryHandler(logger, _promptRepository);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task Handle_ShouldThrow_WhenPromptNotFound()
	{
		// Arrange
		var query = _fixture.Create<GetPromptQuery>();
		
		// Act
		Func<Task> act = () => _sut.Handle(query, CancellationToken.None);
		
		// Assert
		await act.Should().ThrowAsync<PromptNotFoundException>();
	}
	
	[Fact]
	public async Task Handle_ShouldThrow_WhenPromptFoundButNotOwnedByRequesterOrSystem()
	{
		// Arrange
		var prompt = _fixture.Create<Prompt>();
		var query = _fixture
			.Build<GetPromptQuery>()
			.With(x => x.PromptId, prompt.PromptId)
			.Create();
		var cancellationToken = CancellationToken.None;

		_promptRepository
			.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(prompt);

		// Act
		Func<Task> act = () => _sut.Handle(query, cancellationToken);

		// Assert
		await act.Should().ThrowAsync<PromptAuthorizationException>();
	}
	
	[Fact]
	public async Task Handle_ShouldReturn_WhenPromptFoundAndOwnedByRequester()
	{
		// Arrange
		var prompt = _fixture.Create<Prompt>();
		var query = new GetPromptQuery{ PromptId = prompt.PromptId, UserId = prompt.OwnerId };
		var cancellationToken = CancellationToken.None;

		_promptRepository
			.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(prompt);

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().Be(prompt);
	}
	
	[Fact]
	public async Task Handle_ShouldReturn_WhenPromptFoundAndOwnedBySystem()
	{
		// Arrange
		var prompt = Prompt.CreateNew(
			_fixture.Create<string>(),
			_fixture.Create<string>(),
			Constants.SystemUser);
		var query = _fixture
			.Build<GetPromptQuery>()
			.With(x => x.PromptId, prompt.PromptId)
			.Create();
		var cancellationToken = CancellationToken.None;

		_promptRepository
			.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
			.Returns(prompt);

		// Act
		var result = await _sut.Handle(query, cancellationToken);

		// Assert
		result.Should().NotBeNull();
		result.Should().Be(prompt);
	}
}

