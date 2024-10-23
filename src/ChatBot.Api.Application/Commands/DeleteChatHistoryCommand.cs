using MediatR;

namespace ChatBot.Api.Application.Commands;

public record DeleteChatHistoryCommand : IRequest
{
    public required Guid ContextId { get; init; }

    public required string Username { get; init; }
}

