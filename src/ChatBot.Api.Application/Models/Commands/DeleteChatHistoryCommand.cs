using MediatR;

namespace ChatBot.Api.Application.Models.Commands;

public record DeleteChatHistoryCommand : IRequest
{
    public required Guid ContextId { get; init; }

    public required string Username { get; init; }
}

