using System.Collections.ObjectModel;
using ChatBot.Domain.ChatContextEntity;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;

public class ChatContextEntityFrameworkRepository(ChatBotDbContext dbContext) : IChatContextRepository
{
    private readonly ChatBotDbContext _dbContext = dbContext;
    public async Task<ChatContext?> GetAsync(Guid contextId, CancellationToken cancellationToken)
    {
        return await _dbContext.ChatContexts
            .Include("_messages")
            .SingleOrDefaultAsync(x => x.ContextId == contextId, cancellationToken);    
    }

    public async Task<ReadOnlyCollection<ChatContextMetadata>> GetManyMetadataAsync(Guid userId, CancellationToken cancellationToken)
    {
        var metadatas = await _dbContext.ChatContexts
            .Where(x => x.UserId == userId)
            .Select(x => ChatContextMetadata.CreateExisting(x.ContextId, x.Title, x.UserId, x.CreatedAt, x.UpdatedAt))
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