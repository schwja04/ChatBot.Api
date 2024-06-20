using MediatR;

namespace ChatBot.Api.Application.Models.Commands;

public record UpdatePromptCommand : IRequest
{
    public required Prompt Prompt { get; init; }
}
