using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;

internal class PromptMessageMapper(IPromptRepository promptRepository) : IPromptMessageMapper
{
    private readonly IReadPromptRepository _readPromptRepository = promptRepository;

    public async Task<string> BuildMessageContentAsync(ChatMessage message, string username, CancellationToken cancellationToken)
    {
        if (string.Equals(message.PromptKey, PromptKey.None, StringComparison.OrdinalIgnoreCase))
        {
            return message.Content;
        }

        Prompt? prompt = await _readPromptRepository.GetAsync(username, message.PromptKey, cancellationToken);

        if (prompt is null)
        {
            return message.Content;
        }

        return string.Format(prompt.Value, message.Content);
    }
}