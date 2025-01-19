using System.Collections.ObjectModel;
using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetManyPrompts;

internal class GetManyPromptsQueryHandler(IPromptRepository promptRepository)
	: IRequestHandler<GetManyPromptsQuery, ReadOnlyCollection<Prompt>>
{
	private readonly IReadPromptRepository _readPromptRepository = promptRepository;

    public async Task<ReadOnlyCollection<Prompt>> Handle(GetManyPromptsQuery request, CancellationToken cancellationToken)
    {
	    var userPrompts = await _readPromptRepository.GetManyAsync(request.Username, cancellationToken);
	    
	    if (!request.IncludeSystemPrompts)
	    {
		    return userPrompts;
	    }
	    
	    var systemPrompts = await _readPromptRepository.GetManyAsync(Constants.SystemUser, cancellationToken);
	    
	    var allPrompts = new List<Prompt>(userPrompts);
	    allPrompts.AddRange(systemPrompts);
	    return allPrompts.AsReadOnly();
    }
}

