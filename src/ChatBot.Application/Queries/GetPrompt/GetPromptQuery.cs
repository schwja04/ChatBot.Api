using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetPrompt;

public record GetPromptQuery(string Username, Guid PromptId) 
    : IRequest<Prompt>;