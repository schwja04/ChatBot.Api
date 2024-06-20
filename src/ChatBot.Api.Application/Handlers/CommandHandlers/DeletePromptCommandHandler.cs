using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Commands;

namespace ChatBot.Api.Application.Handlers.CommandHandlers;

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
