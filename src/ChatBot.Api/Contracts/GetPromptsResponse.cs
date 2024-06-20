using ChatBot.Api.Application.Models;

namespace ChatBot.Api.Contracts;

public record GetPromptsResponse
{
    public required IReadOnlyCollection<Prompt> Prompts { get; init; }
}

