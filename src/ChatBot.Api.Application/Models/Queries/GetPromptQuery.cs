﻿using MediatR;

namespace ChatBot.Api.Application.Models.Queries;

public record GetPromptQuery : IRequest<Prompt?>
{
    public GetPromptQuery(string username, Guid promptId)
    {
        Username = username;
        PromptId = promptId;
    }

    public GetPromptQuery(string username, string promptKey)
    {
        Username = username;
        PromptKey = promptKey;
    }

    public string Username { get; }

    public Guid? PromptId { get; }

    public string? PromptKey { get; }
}