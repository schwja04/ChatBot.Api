using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Queries;
using ChatBot.Api.Application.Models;
using MediatR;
using System.Collections.ObjectModel;

namespace ChatBot.Api.Application.Handlers.QueryHandlers;

internal class GetPromptsQueryHandler : IRequestHandler<GetPromptsQuery, ReadOnlyCollection<Prompt>>
{
	private readonly IReadPromptRepository _readPromptRepository;

	public GetPromptsQueryHandler(IPromptRepository promptRepository)
	{
		_readPromptRepository = promptRepository;
	}

    public async Task<ReadOnlyCollection<Prompt>> Handle(GetPromptsQuery request, CancellationToken cancellationToken)
    {
		return await _readPromptRepository.GetManyAsync(request.Username, cancellationToken);
    }
}

