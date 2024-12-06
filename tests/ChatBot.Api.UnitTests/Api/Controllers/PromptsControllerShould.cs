using System.Security.Principal;
using AutoFixture;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Application.Commands.CreatePrompt;
using ChatBot.Api.Application.Commands.DeletePrompt;
using ChatBot.Api.Application.Commands.UpdatePrompt;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Application.Queries.GetManyPrompts;
using ChatBot.Api.Application.Queries.GetPrompt;
using ChatBot.Api.Contracts;
using ChatBot.Api.Controllers;
using ChatBot.Api.Domain.PromptEntity;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ChatBot.Api.UnitTests.Api.Controllers;

public class PromptsControllerShould
{
	private readonly IMediator _mediator;
    private readonly ILogger<PromptsController> _logger;

	private readonly PromptsController _sut;

	private readonly IFixture _fixture;

	public PromptsControllerShould()
	{
		_mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<PromptsController>>();
		_sut = new PromptsController(_logger, _mediator);

		_fixture = new Fixture();
	}

	[Fact]
	public async Task GetManyAsync_ShouldReturn200()
	{
		// Arrange
		var username = _fixture.Create<string>();
		var prompts = new List<Prompt>().AsReadOnly();
		var cancellationToken = CancellationToken.None;

		_mediator.Send(Arg.Any<GetManyPromptsQuery>(), Arg.Any<CancellationToken>())
			.Returns(prompts);

		_sut.ControllerContext = CreateTestControllerContext(username);

		// Act
		var response = await _sut.GetManyAsync(cancellationToken);

		// Assert
		var okResponse = (response as OkObjectResult);
		var getPromptsResponse = (okResponse!.Value as GetManyPromptsResponse);
        getPromptsResponse!.Prompts.Should().BeEquivalentTo(prompts!);

	}

    [Fact]
    public async Task GetAsync_ShouldReturn200_WhenPromptFound()
    {
        // Arrange
		var prompt = Prompt.CreateExisting(
			_fixture.Create<Guid>(),
			_fixture.Create<string>(),
			_fixture.Create<string>(),
			_fixture.Create<string>());
        var cancellationToken = CancellationToken.None;

        _mediator.Send(Arg.Any<GetPromptQuery>(), Arg.Any<CancellationToken>())
            .Returns(prompt);

        _sut.ControllerContext = CreateTestControllerContext(prompt.Owner);

        // Act
        var response = await _sut.GetAsync(prompt.PromptId, cancellationToken);

        // Assert
        var okResponse = (response as OkObjectResult);
        var getPromptResponse = (okResponse!.Value as Prompt);
        getPromptResponse!.Should().Be(prompt);

    }

    [Fact]
    public async Task GetAsync_ShouldReturn404_WhenPromptNotReturnedFromMediator()
    {
        // Arrange
        var username = _fixture.Create<string>();
        var promptId = _fixture.Create<Guid>();
        var cancellationToken = CancellationToken.None;

        _mediator.Send(Arg.Any<GetPromptQuery>(), Arg.Any<CancellationToken>())
            .Returns((Prompt?)null);

        _sut.ControllerContext = CreateTestControllerContext(username);

        // Act
        var response = await _sut.GetAsync(promptId, cancellationToken);

        // Assert
        var notFoundResponse = (response as NotFoundResult);
        notFoundResponse.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnPromptWhenSaved()
    {
        // Arrange
        var request = _fixture.Create<CreatePromptRequest>();
        var username = _fixture.Create<string>();
        var cancellationToken = CancellationToken.None;

        var prompt = Prompt.CreateNew(
            request.Key,
            request.Value,
            username);

        _mediator.Send(Arg.Any<CreatePromptCommand>(), Arg.Any<CancellationToken>())
            .Returns(prompt);

        _sut.ControllerContext = CreateTestControllerContext(username);

        // Act
        var result = await _sut.CreateAsync(request, cancellationToken);

        // Assert
        var createResult = (result as CreatedResult);
        var createPromptResponse = (createResult!.Value as CreatePromptResponse);
        createPromptResponse!.Prompt.Should().Be(prompt);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent()
    {
        // Arrange
        var username = _fixture.Create<string>();
        var promptId = _fixture.Create<Guid>();

        var cancellationToken = CancellationToken.None;

        _sut.ControllerContext = CreateTestControllerContext(username);

        // Act
        var result = await _sut.DeleteAsync(promptId, cancellationToken);

        // Assert
        var noContentResult = (result as NoContentResult);
        noContentResult.Should().NotBeNull();

        await _mediator
            .Received(1)
            .Send(
                Arg.Is<DeletePromptCommand>(x =>
                    x.PromptId == promptId
                    && x.Username == username
                ),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNoContent()
    {
        // Arrange
        var username = _fixture.Create<string>();
        var request = _fixture.Create<UpdatePromptRequest>();
        var promptId = _fixture.Create<Guid>();

        var cancellationToken = CancellationToken.None;

        _sut.ControllerContext = CreateTestControllerContext(username);

        // Act
        var result = await _sut.UpdateAsync(promptId, request, cancellationToken);

        // Assert
        var noContentResult = (result as NoContentResult);
        noContentResult.Should().NotBeNull();

        await _mediator
            .Received(1)
            .Send(
                Arg.Is<UpdatePromptCommand>(x =>
                    x.PromptId == promptId
                    && x.Key == request.Key
                    && x.Value == request.Value
                    && x.Owner == username),
                Arg.Any<CancellationToken>());
    }

    private static ControllerContext CreateTestControllerContext(string username)
	{
		return new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
			{
				User = new GenericPrincipal(new GenericIdentity(username), (string[]?)null)
            }
        };
    }
}