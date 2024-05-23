namespace ChatBot.Api.Application.Models.Queries;

public record GetChatHistoryQueryResponse
{
    public required ChatHistory ChatHistory { get; init; }
}

