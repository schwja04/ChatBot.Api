namespace Common.OpenAI.Models;

public class CreateChatCompletionResponse
{
    public string Id { get; set; } = null!;
    public Choice[] Choices { get; set; } = null!;
    public int Created { get; set; }
    public string Model { get; set; } = null!;
    public string SystemFingerprint { get; set; } = null!;
    public string Object { get; set; } = null!;
    public CompletionUsage Usage { get; set; } = null!;
}

public class CompletionUsage
{
    public int CompletionTokens { get; set; }
    public int PromptTokens { get; set; }
    public int TotalTokens { get; set; }
}

public class Choice
{
    public int Index { get; set; }
    public string FinishReason { get; set; } = null!;
    public ChatCompletionResponseMessage Message { get; set; } = null!;
}

public class ChatCompletionResponseMessage
{
    public string Content { get; set; } = null!;
    public string Role { get; set; } = null!;
    public ChatCompletionMessageToolCall[] ToolCalls { get; set; } = null!;
}

public class ChatCompletionMessageToolCall
{
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    public ChatCompletionFunction Function { get; set; } = null!;
}

public class ChatCompletionFunction
{
    public string Name { get; set; } = null!;
    public string Arguments { get; set; } = null!;
}
