using ChatBot.Api.Contracts;
using ChatBot.Domain.PromptEntity;

namespace ChatBot.Api.Mappers;

internal static class PromptMapper
{
    public static CreatePromptResponse ToCreatePromptResponse(this Prompt prompt)
    {
        return new CreatePromptResponse
        {
            PromptId = prompt.PromptId,
            Key = prompt.Key,
            Value = prompt.Value,
            OwnerId = prompt.OwnerId
        };
    }
    
    public static GetPromptResponse ToGetPromptResponse(this Prompt prompt)
    {
        return new GetPromptResponse
        {
            PromptId = prompt.PromptId,
            Key = prompt.Key,
            Value = prompt.Value,
            OwnerId = prompt.OwnerId
        };
    }
}