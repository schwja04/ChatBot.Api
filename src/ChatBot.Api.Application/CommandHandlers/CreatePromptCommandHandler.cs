﻿using MediatR;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Application.CommandHandlers;

internal class CreatePromptCommandHandler(IPromptRepository promptRepository)
    : IRequestHandler<CreatePromptCommand, Prompt>
{
    private readonly IWritePromptRepository _writePromptRepository = promptRepository;

    public async Task<Prompt> Handle(
        CreatePromptCommand request, CancellationToken cancellationToken)
    {
        Prompt newPrompt = Prompt.CreateNew(request.Key, request.Value, request.Owner);

        await _writePromptRepository.SaveAsync(newPrompt, cancellationToken);

        return newPrompt;
    }
}
