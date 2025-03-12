using ChatBot.Domain.PromptEntity;
using ChatBot.Infrastructure.Repositories.Persistence.Mongo.Models;

namespace ChatBot.Infrastructure.Repositories.Persistence.Mongo.Mappers;

internal static class PromptMapper
{
    public static PromptDal ToDal(this Prompt prompt)
    {
        return new PromptDal
        {
            PromptId = prompt.PromptId,
            Key = prompt.Key,
            Value = prompt.Value,
            OwnerId = prompt.OwnerId,
        };
    }

    public static Prompt ToDomain(this PromptDal prompt)
    {
        return Prompt.CreateExisting(
            promptId: prompt.PromptId,
            key: prompt.Key,
            value: prompt.Value,
            ownerId: prompt.OwnerId);
    }
}
