using MediatR;

namespace ChatBot.Api.Application.Models.Commands;

public record ProcessChatMessageCommand : IRequest<ProcessChatMessageCommandResponse>
{
    public Guid ContextId { get; init; }

    public required string Username { get; init; }

    public required string Content { get; init; }
}

