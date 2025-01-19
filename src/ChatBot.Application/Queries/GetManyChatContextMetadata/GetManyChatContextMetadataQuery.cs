using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetManyChatContextMetadata;

public record GetManyChatContextMetadataQuery : IRequest<ReadOnlyCollection<ChatContextMetadata>>
{
    public required string Username { get; init; }
}