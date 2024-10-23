using MediatR;
using ChatBot.Api.Application.Commands;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Application.CommandHandlers;

internal class DeletePromptCommandHandler : IRequestHandler<DeletePromptCommand>
{
    private readonly IWritePromptRepository _writePromptRepository;

    public DeletePromptCommandHandler(IPromptRepository promptRepository)
    {
        _writePromptRepository = promptRepository;
    }
    
    public async Task Handle(DeletePromptCommand request, CancellationToken cancellationToken)
    {
        await _writePromptRepository.DeleteAsync(request.Username, request.PromptId, cancellationToken);
    }
}
