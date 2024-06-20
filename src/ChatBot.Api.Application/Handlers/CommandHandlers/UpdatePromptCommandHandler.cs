using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Commands;

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

		await _writePromptRepository.SaveAsync(request.Prompt, cancellationToken);
    }
}
