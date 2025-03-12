using MediatR;

namespace ChatBot.Application.Commands.DeleteChatContext;

public record DeleteChatContextCommand : IRequest
{
    public required Guid ContextId { get; init; }

    public required Guid UserId { get; init; }
}

