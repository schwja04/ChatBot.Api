using System.ComponentModel.DataAnnotations;

namespace ChatBot.Api.Contracts;

public record ProcessChatMessageRequest
{
    public Guid ContextId { get; init; }

    [Required]
    public string Content { get; init; } = null!;

    public string PromptKey { get; init; } = Domain.PromptEntity.PromptKey.None;
}

public record ProcessChatMessageResponse
{
    public required Guid ContextId { get; init; }

    public required ChatMessageResponse ChatMessage { get; init; }
}