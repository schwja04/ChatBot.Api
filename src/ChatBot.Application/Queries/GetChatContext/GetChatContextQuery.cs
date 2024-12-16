using ChatBot.Api.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetChatContext;

public record GetChatContextQuery : IRequest<ChatContext>
{
    public required Guid ContextId { get; init; }

    public required string Username { get; init; }
}