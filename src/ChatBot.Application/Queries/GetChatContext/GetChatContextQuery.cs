using ChatBot.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetChatContext;

public record GetChatContextQuery : IRequest<ChatContext>
{
    public required Guid ContextId { get; init; }

    public required Guid UserId { get; init; }
}