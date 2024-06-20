using MediatR;

namespace ChatBot.Api.Application.Models.Commands;

public record ProcessChatMessageCommand : IRequest<ProcessChatMessageCommandResponse>
{
    public Guid ContextId { get; init; }

    public required string Username { get; init; }

    public required string Content { get; init; }

    public required string PromptKey { get; init; }
}

public record ProcessChatMessageCommandResponse
{
    public required Guid ContextId { get; init; }

    public required ChatMessage ChatMessage { get; init; }
}