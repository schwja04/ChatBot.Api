using MediatR;
using System.Collections.ObjectModel;
using ChatBot.Api.Application.Queries;
using ChatBot.Api.Domain.PromptEntity;

namespace ChatBot.Api.Application.QueryHandlers;

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

