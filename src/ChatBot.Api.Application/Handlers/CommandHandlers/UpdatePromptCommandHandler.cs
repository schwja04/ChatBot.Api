using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Commands;
using ChatBot.Api.Application.Models;

namespace ChatBot.Api.Application.Handlers.CommandHandlers;

internal class UpdatePromptCommandHandler : IRequestHandler<UpdatePromptCommand>
{
    private readonly IWritePromptRepository _writePromptRepository;

    public UpdatePromptCommandHandler(IPromptRepository promptRepository)
	{
		_writePromptRepository = promptRepository;
	}

    public async Task Handle(UpdatePromptCommand request, CancellationToken cancellationToken)
    {
        Prompt prompt = Prompt.CreateExisting(
            request.PromptId,
            request.Key,
            request.Value,
            request.Owner);
		await _writePromptRepository.SaveAsync(prompt, cancellationToken);
    }
}
