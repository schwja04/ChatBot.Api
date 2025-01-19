using System.Collections.ObjectModel;
using ChatBot.Domain.PromptEntity;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;

internal class PromptEntityFrameworkRepository(ChatBotDbContext dbContext) : IPromptRepository
{
    private readonly ChatBotDbContext _dbContext = dbContext;
    
    public async Task<Prompt?> GetAsync(Guid promptId, CancellationToken cancellationToken)
    {
        var prompt =await _dbContext.Prompts
            .SingleOrDefaultAsync(p => p.PromptId == promptId, cancellationToken);
        
        return prompt;
    }

    public async Task<Prompt?> GetAsync(string username, string promptKey, CancellationToken cancellationToken)
    {
        var prompt = await _dbContext.Prompts
            .SingleOrDefaultAsync(p => p.Owner == username && p.Key == promptKey, cancellationToken);
        return prompt;
    }

    public async Task<ReadOnlyCollection<Prompt>> GetManyAsync(string username, CancellationToken cancellationToken)
    {
        var prompts =  await _dbContext.Prompts
            .Where(p => p.Owner == username)
            .ToListAsync(cancellationToken);

        return prompts.AsReadOnly();
    }

    public async Task CreateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        _dbContext.Prompts.Add(prompt);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        _dbContext.Prompts.Remove(prompt);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        _dbContext.Prompts.Update(prompt);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}