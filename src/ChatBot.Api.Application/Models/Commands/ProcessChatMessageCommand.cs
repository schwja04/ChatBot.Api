using MediatR;

namespace ChatBot.Api.Application.Models.Commands;

public record ProcessChatMessageCommand : IRequest<ProcessChatMessageCommandResponse>
{
    public Guid ContextId { get; init; }

    public string Content { get; init; } = null!;
}

