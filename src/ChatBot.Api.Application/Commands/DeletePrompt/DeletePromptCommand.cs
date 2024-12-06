using MediatR;

namespace ChatBot.Api.Application.Commands.DeletePrompt;

public record DeletePromptCommand : IRequest
{
    public required string Username { get; init; }
    public required Guid PromptId { get; init; }
}
