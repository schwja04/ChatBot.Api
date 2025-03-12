using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;

internal class ChatMessageMapper(IPromptMessageMapper promptMapper) 
    : IChatMessageMapper
{
    private readonly IPromptMessageMapper _promptMapper = promptMapper;
    
    public async Task<ReadOnlyCollection<ChatMessage>> ToLLMChatMessagesAsync(
        ChatContext chatContext,
        CancellationToken cancellationToken)
    {
        List<ChatMessage> reqMessages = new(chatContext.Messages.Count);

        Domain.ChatContextEntity.ChatMessage currentMessage;
        for (int i = 0; i < chatContext.Messages.Count - 1; i++)
        {
            currentMessage = chatContext.Messages[i];
            reqMessages.Add(new ChatMessage
            {
                Role = currentMessage.Role.ToLLMChatRole(),
                Text = currentMessage.Content
            });
        }
        
        currentMessage = chatContext.Messages[^1];
        reqMessages.Add(new ChatMessage
        {
            Role = currentMessage.Role.ToLLMChatRole(),
            Text = await _promptMapper.BuildMessageContentAsync(
                currentMessage, chatContext.UserId, cancellationToken)
        });
        
        return reqMessages.AsReadOnly();
    }
}