using ChatBot.Api.Domain.ChatHistoryEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries;

public record GetChatHistoryQuery : IRequest<GetChatHistoryQueryResponse>
{
    public required Guid ContextId { get; init; }

    public required string Username { get; init; }
}

public record GetChatHistoryQueryResponse
{
    public required ChatHistory ChatHistory { get; init; }
}