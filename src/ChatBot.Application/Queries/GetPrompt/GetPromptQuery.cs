using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetPrompt;

public record GetPromptQuery(string Username, Guid PromptId) 
    : IRequest<Prompt>;