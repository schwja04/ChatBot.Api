using System.Collections.ObjectModel;
using MediatR;

namespace ChatBot.Api.Application.Models.Queries;

public record GetChatHistoryMetadatasQuery : IRequest<GetChatHistoryMetadatasQueryResponse>
{
    public required string UserName { get; init; }
}

public record GetChatHistoryMetadatasQueryResponse
{
    public required ReadOnlyCollection<ChatHistoryMetadata> ChatHistoryMetadatas { get; init; }
}