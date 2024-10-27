using System.Collections.ObjectModel;
using ChatBot.Api.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries;

public record GetManyChatContextMetadataQuery : IRequest<GetChatHistoryMetadatasQueryResponse>
{
    public required string UserName { get; init; }
}

public record GetChatHistoryMetadatasQueryResponse
{
    public required ReadOnlyCollection<ChatContextMetadata> ChatContextMetadatas { get; init; }
}