using System.Collections.ObjectModel;
using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetManyPrompts;

public record GetManyPromptsQuery : IRequest<ReadOnlyCollection<Prompt>>
{
	public required string Username { get; init; }
	
	public required bool IncludeSystemPrompts { get; init; }
}