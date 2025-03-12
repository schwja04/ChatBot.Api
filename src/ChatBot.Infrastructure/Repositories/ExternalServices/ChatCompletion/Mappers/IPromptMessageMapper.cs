using ChatBot.Domain.ChatContextEntity;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;

internal interface IPromptMessageMapper
{
    Task<string> BuildMessageContentAsync(
        ChatMessage message, Guid userId, CancellationToken cancellationToken);
}