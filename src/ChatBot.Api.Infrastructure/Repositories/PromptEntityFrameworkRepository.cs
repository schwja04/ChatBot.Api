using System.Collections.ObjectModel;
using ChatBot.Api.Domain.PromptEntity;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Api.Infrastructure.Repositories;

internal class PromptEntityFrameworkRepository(ChatBotContext dbContext) : IPromptRepository
{
    private readonly ChatBotContext _dbContext = dbContext;
    
    public async Task<Prompt?> GetAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        return await _dbContext.Prompts
            .SingleOrDefaultAsync(p => p.PromptId == promptId && p.Owner == username, cancellationToken);
    }

    public async Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken)
    {
        return await _dbContext.Prompts
            .SingleOrDefaultAsync(p => p.Owner == username && p.Key == promptKey, cancellationToken);
    }

    public async Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken)
    {
        var prompts =  await _dbContext.Prompts
            .Where(p => p.Owner == username)
            .ToListAsync(cancellationToken);

        return prompts.AsReadOnly();
    }

    public async Task DeleteAsync(string username, Guid promptId, CancellationToken cancellationToken)
    {
        Prompt? prompt = await GetAsync(username, promptId, cancellationToken);
        if (prompt is null)
        {
            return;
        }
        
        _dbContext.Prompts.Remove(prompt);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        if (_dbContext.Prompts.Entry(prompt).State == EntityState.Detached)
        {
            _dbContext.Prompts.Add(prompt);
        }
        else
        {
            _dbContext.Prompts.Update(prompt);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}