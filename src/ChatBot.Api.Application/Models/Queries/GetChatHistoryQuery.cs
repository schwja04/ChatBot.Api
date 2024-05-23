using MediatR;

namespace ChatBot.Api.Application.Models.Queries;

public record GetChatHistoryQuery : IRequest<GetChatHistoryQueryResponse>
{
    public Guid ContextId { get; init; }
}
