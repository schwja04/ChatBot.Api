using MediatR;

namespace ChatBot.Application.Commands.UpdateChatContextTitle;

public record UpdateChatContextTitleCommand : IRequest
{
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }

    public required Guid UserId { get; init; }
}
