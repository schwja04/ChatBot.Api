using ChatBot.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Application.Commands.ProcessChatMessage;

public record ProcessChatMessageCommand : IRequest<ProcessChatMessageCommandResponse>
{
    public Guid ContextId { get; init; }

    public required Guid UserId { get; init; }

    public required string Content { get; init; }

    public required string PromptKey { get; init; }
}

public record ProcessChatMessageCommandResponse
{
    public required Guid ContextId { get; init; }

    public required ChatMessage ChatMessage { get; init; }
}