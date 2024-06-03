using MediatR;

namespace ChatBot.Api.Application.Models.Commands;

public record ProcessChatMessageTitleCommand : IRequest
{
    public required Guid ContextId { get; init; }

    public required string Title { get; init; }

    public required string Username { get; init; }
}
