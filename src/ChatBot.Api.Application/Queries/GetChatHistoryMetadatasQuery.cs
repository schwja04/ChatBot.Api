using System.Collections.ObjectModel;
using ChatBot.Api.Domain.ChatHistoryEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries;

public record GetChatHistoryMetadatasQuery : IRequest<GetChatHistoryMetadatasQueryResponse>
{
    public required string UserName { get; init; }
}

public record GetChatHistoryMetadatasQueryResponse
{
    public required ReadOnlyCollection<ChatHistoryMetadata> ChatHistoryMetadatas { get; init; }
}