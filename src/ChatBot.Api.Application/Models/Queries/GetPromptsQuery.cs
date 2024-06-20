using System.Collections.ObjectModel;
using MediatR;

namespace ChatBot.Api.Application.Models.Queries;

public record GetPromptsQuery : IRequest<GetPromptsQueryResponse>
{
	public required string Username { get; init; }
}

public class GetPromptsQueryResponse
{
	public required ReadOnlyCollection<Prompt> Prompts { get; init; }
}
