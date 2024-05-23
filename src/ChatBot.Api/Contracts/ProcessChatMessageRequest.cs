using System.ComponentModel.DataAnnotations;

namespace ChatBot.Api.Contracts;

public record ProcessChatMessageRequest
{
    public Guid ContextId { get; init; }

    [Required]
    public string Content { get; init; } = null!;
}
