namespace Common.OpenAI.Models;

public class CreateChatCompletionRequest
{
    public ChatCompletionRequestMessage[] Messages { get; set; } = null!;
    public string Model { get; set; } = null!;
    public double FrequencyPenalty { get; set; }
    public int MaxTokens { get; set; }
    public int ChoiceCount { get; set; } = 1;
    public double PresencePenalty { get; set; }
    public ResponseFormat ResponseFormat { get; set; } = null!;
    public bool Stream { get; set; }
    public double Temperature { get; set; }
    public double TopP { get; set; }
}

public class ChatCompletionRequestMessage
{
    public string Content { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public class ResponseFormat
{
    public string Type { get; set; } = "text"; // "text" or "json_object"
}
