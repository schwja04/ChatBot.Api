using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetPrompt;

public record GetPromptQuery
    : IRequest<Prompt>
{
    public required Guid PromptId { get; init; }
    public required Guid UserId { get; init; }
};