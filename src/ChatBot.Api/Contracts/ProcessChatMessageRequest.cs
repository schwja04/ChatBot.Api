using System.ComponentModel.DataAnnotations;

using AppModels = ChatBot.Api.Application.Models;

namespace ChatBot.Api.Contracts;

public record ProcessChatMessageRequest
{
    public Guid ContextId { get; init; }

    [Required]
    public string Content { get; init; } = null!;

    public string PromptKey { get; init; } = AppModels.PromptKey.None;
}

public record ProcessChatMessageResponse
{
    public required Guid ContextId { get; init; }

    public required ChatMessageResponse ChatMessage { get; init; }
}