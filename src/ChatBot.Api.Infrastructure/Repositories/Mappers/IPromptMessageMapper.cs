using ChatBot.Api.Application.Models;

namespace ChatBot.Api.Infrastructure.Repositories.Mappers;

internal interface IPromptMessageMapper
{
    Task<string> BuildMessageContentAsync(ChatMessage message, string username, CancellationToken cancellationToken);
}