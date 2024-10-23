using MediatR;

namespace ChatBot.Api.Application.Commands;

public record UpdateChatMessageTitleCommand : IRequest
{
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }

    public required string Username { get; init; }
}
