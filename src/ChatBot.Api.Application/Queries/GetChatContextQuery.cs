using ChatBot.Api.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries;

public record GetChatContextQuery : IRequest<GetChatContextQueryResponse>
{
    public required Guid ContextId { get; init; }

    public required string Username { get; init; }
}

public record GetChatContextQueryResponse
{
    public required ChatContext ChatContext { get; init; }
}