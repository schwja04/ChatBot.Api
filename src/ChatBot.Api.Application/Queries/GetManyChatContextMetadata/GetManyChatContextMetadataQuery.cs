using System.Collections.ObjectModel;
using ChatBot.Api.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetManyChatContextMetadata;

public record GetManyChatContextMetadataQuery : IRequest<ReadOnlyCollection<ChatContextMetadata>>
{
    public required string Username { get; init; }
}