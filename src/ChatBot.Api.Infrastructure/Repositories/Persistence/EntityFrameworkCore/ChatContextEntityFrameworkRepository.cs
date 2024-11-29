using System.Collections.ObjectModel;
using ChatBot.Api.Domain.ChatContextEntity;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Api.Infrastructure.Repositories.Persistence.EntityFrameworkCore;

public class ChatContextEntityFrameworkRepository(ChatBotContext dbContext) : IChatContextRepository
{
    private readonly ChatBotContext _dbContext = dbContext;
    public async Task<ChatContext?> GetAsync(Guid contextId, CancellationToken cancellationToken)
    {
        return await _dbContext.ChatContexts
            .Include("_messages")
            .SingleOrDefaultAsync(x => x.ContextId == contextId, cancellationToken);    
    }

    public async Task<ReadOnlyCollection<ChatContextMetadata>> GetManyMetadataAsync(string username, CancellationToken cancellationToken)
    {
        var metadatas = await _dbContext.ChatContexts
            .Where(x => x.Username == username)
            .Select(x => ChatContextMetadata.CreateExisting(x.ContextId, x.Title, x.Username, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(cancellationToken);
        
        return metadatas.AsReadOnly();
    }

    public async Task SaveAsync(ChatContext chatContext, CancellationToken cancellationToken)
    {
        if (_dbContext.ChatContexts.Entry(chatContext).State == EntityState.Detached)
        {
            _dbContext.ChatContexts.Add(chatContext);
        }
        else
        {
            _dbContext.ChatContexts.Update(chatContext);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid contextId, CancellationToken cancellationToken)
    {
        var chatContext = await GetAsync(contextId, cancellationToken);
        if (chatContext is null)
        {
            return;
        }

        _dbContext.ChatContexts.Remove(chatContext);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}