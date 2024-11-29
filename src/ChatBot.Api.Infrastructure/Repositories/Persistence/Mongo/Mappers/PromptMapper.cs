using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Models;

namespace ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Mappers;

internal static class PromptMapper
{
    public static PromptDal ToDal(this Prompt prompt)
    {
        return new PromptDal
        {
            PromptId = prompt.PromptId,
            Key = prompt.Key,
            Value = prompt.Value,
            Owner = prompt.Owner,
        };
    }

    public static Prompt ToDomain(this PromptDal prompt)
    {
        return Prompt.CreateExisting(
            promptId: prompt.PromptId,
            key: prompt.Key,
            value: prompt.Value,
            owner: prompt.Owner);
    }
}
