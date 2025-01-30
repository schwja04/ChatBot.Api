namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion;

internal record ChatCompletionOptions
{
    public required string Model { get; init; }
}