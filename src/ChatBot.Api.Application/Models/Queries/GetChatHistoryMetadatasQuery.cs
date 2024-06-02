using MediatR;

namespace ChatBot.Api.Application.Models.Queries;

public record GetChatHistoryMetadatasQuery : IRequest<GetChatHistoryMetadatasQueryResponse>
{
    public required string UserName { get; init; }
}
