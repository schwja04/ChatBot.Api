using MediatR;

namespace ChatBot.Api.Application.Models.Queries;

public record GetChatHistoryQuery : IRequest<GetChatHistoryQueryResponse>
{
    public required Guid ContextId { get; init; }

    public required string Username { get; init; }
}
