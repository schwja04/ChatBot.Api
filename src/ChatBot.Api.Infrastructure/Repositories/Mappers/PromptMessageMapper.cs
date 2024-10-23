using ChatBot.Api.Domain.ChatHistoryEntity;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Infrastructure.Repositories.Mappers;

internal class PromptMessageMapper : IPromptMessageMapper
{
    private readonly IReadPromptRepository _readPromptRepository;

    public PromptMessageMapper(IPromptRepository promptRepository)
    {
        _readPromptRepository = promptRepository;
    }

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