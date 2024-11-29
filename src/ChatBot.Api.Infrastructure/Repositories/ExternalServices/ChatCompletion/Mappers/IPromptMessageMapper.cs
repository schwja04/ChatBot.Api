using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;

internal interface IPromptMessageMapper
{
    Task<string> BuildMessageContentAsync(ChatMessage message, string username, CancellationToken cancellationToken);
}