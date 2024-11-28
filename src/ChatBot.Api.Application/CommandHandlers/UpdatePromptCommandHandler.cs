using MediatR;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Application.CommandHandlers;

internal class UpdatePromptCommandHandler(IPromptRepository promptRepository)
	: IRequestHandler<UpdatePromptCommand>
{
    private readonly IWritePromptRepository _writePromptRepository = promptRepository;

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
