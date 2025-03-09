namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion;

internal record ChatCompletionOptions
{
    public required string DefaultModel { get; init; } = "llama3.2";
    public required float DefaultFrequencyPenalty { get; init; } = 0.0F;
    public required int DefaultMaxTokens { get; init; } = 100;
    public required float DefaultPresencePenalty { get; init; } = 0.0F;
    public required float DefaultTemperature { get; init; } = 1.0F;
    public required float DefaultTopP { get; init; } = 1.0F;
}