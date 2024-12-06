using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Commands.DeletePrompt;

internal class DeletePromptCommandHandler(IPromptRepository promptRepository)
    : IRequestHandler<DeletePromptCommand>
{
    private readonly IWritePromptRepository _writePromptRepository = promptRepository;
    
    public async Task Handle(DeletePromptCommand request, CancellationToken cancellationToken)
    {
        await _writePromptRepository.DeleteAsync(request.Username, request.PromptId, cancellationToken);
    }
}
