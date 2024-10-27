using ChatBot.Api.Domain.ChatContextEntity;

namespace ChatBot.Api.Application.Abstractions.Repositories;

public interface IChatCompletionRepository
{
    Task<ChatMessage> GetChatCompletionAsync(ChatContext chatContext, CancellationToken cancellationToken);
}
