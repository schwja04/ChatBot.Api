namespace ChatBot.Api.Application.Models.Commands;

public record ProcessChatMessageCommandResponse
{
    public required Guid ContextId { get; init; }

    public required ChatMessage ChatMessage { get; init; }
}
