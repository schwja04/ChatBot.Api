using System.Collections.ObjectModel;
using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetManyPrompts;

internal class GetManyPromptsQueryHandler(IPromptRepository promptRepository)
	: IRequestHandler<GetManyPromptsQuery, ReadOnlyCollection<Prompt>>
{
	private readonly IReadPromptRepository _readPromptRepository = promptRepository;

    public async Task<ReadOnlyCollection<Prompt>> Handle(GetManyPromptsQuery request, CancellationToken cancellationToken)
    {
		return await _readPromptRepository.GetManyAsync(request.Username, cancellationToken);
    }
}

