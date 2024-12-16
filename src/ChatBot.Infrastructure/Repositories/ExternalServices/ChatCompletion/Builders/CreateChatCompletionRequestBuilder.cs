using Common.OpenAI.Models;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Builders;

internal class CreateChatCompletionRequestBuilder
{
    private string _model = null!;
    private double _frequencyPenalty = CreateChatCompletionRequestDefaults.DefaultFrequencyPenalty;
    private int _maxTokens = CreateChatCompletionRequestDefaults.DefaultMaxTokens;
    private int _choiceCount = CreateChatCompletionRequestDefaults.DefaultChoiceCount;
    private double _presencePenalty = CreateChatCompletionRequestDefaults.DefaultPresencePenalty;
    private ResponseFormat _responseFormat = CreateChatCompletionRequestDefaults.DefaultResponseFormat;
    private bool _stream = CreateChatCompletionRequestDefaults.DefaultStream;
    private double _temperature = CreateChatCompletionRequestDefaults.DefaultTemperature;
    private double _topP = CreateChatCompletionRequestDefaults.DefaultTopP;

    private ChatCompletionRequestMessage[] _messages = null!;

    public CreateChatCompletionRequestBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithFrequencyPenalty(double frequencyPenalty)
    {
        _frequencyPenalty = frequencyPenalty;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithMaxTokens(int maxTokens)
    {
        _maxTokens = maxTokens;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithChoiceCount(int choiceCount)
    {
        _choiceCount = choiceCount;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithPresencePenalty(double presencePenalty)
    {
        _presencePenalty = presencePenalty;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithResponseFormat(ResponseFormat responseFormat)
    {
        _responseFormat = responseFormat;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithStream(bool stream)
    {
        _stream = stream;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithTemperature(double temperature)
    {
        _temperature = temperature;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithTopP(double topP)
    {
        _topP = topP;
        return this;
    }

    public CreateChatCompletionRequestBuilder WithMessages(IEnumerable<ChatCompletionRequestMessage> messages)
    {
        ArgumentNullException.ThrowIfNull(messages, nameof(messages));

        _messages = messages.ToArray();

        return this;
    }

    public CreateChatCompletionRequestBuilder UseDefaultFrequencyPenalty()
    {
        return WithFrequencyPenalty(CreateChatCompletionRequestDefaults.DefaultFrequencyPenalty);
    }

    public CreateChatCompletionRequestBuilder UseDefaultMaxTokens()
    {
        return WithMaxTokens(CreateChatCompletionRequestDefaults.DefaultMaxTokens);
    }

    public CreateChatCompletionRequestBuilder UseDefaultChoiceCount()
    {
        return WithChoiceCount(CreateChatCompletionRequestDefaults.DefaultChoiceCount);
    }

    public CreateChatCompletionRequestBuilder UseDefaultPresencePenalty()
    {
        return WithPresencePenalty(CreateChatCompletionRequestDefaults.DefaultPresencePenalty);
    }

    public CreateChatCompletionRequestBuilder UseDefaultResponseFormat()
    {
        return WithResponseFormat(CreateChatCompletionRequestDefaults.DefaultResponseFormat);
    }

    public CreateChatCompletionRequestBuilder UseDefaultStream()
    {
        return WithStream(CreateChatCompletionRequestDefaults.DefaultStream);
    }

    public CreateChatCompletionRequestBuilder UseDefaultTemperature()
    {
        return WithTemperature(CreateChatCompletionRequestDefaults.DefaultTemperature);
    }

    public CreateChatCompletionRequestBuilder UseDefaultTopP()
    {
        return WithTopP(CreateChatCompletionRequestDefaults.DefaultTopP);
    }

    public CreateChatCompletionRequestBuilder UseAllDefaults()
    {
        return UseDefaultFrequencyPenalty()
            .UseDefaultMaxTokens()
            .UseDefaultChoiceCount()
            .UseDefaultPresencePenalty()
            .UseDefaultResponseFormat()
            .UseDefaultStream()
            .UseDefaultTemperature()
            .UseDefaultTopP();
    }


    public CreateChatCompletionRequest Build()
    {
        if (_messages is null || _messages.Length == 0)
        {
            throw new InvalidOperationException("Messages must be set.");
        }

        if (string.IsNullOrWhiteSpace(_model))
        {
            throw new InvalidOperationException("Model must be set.");
        }

        return new CreateChatCompletionRequest()
        {
            Model = _model,
            FrequencyPenalty = _frequencyPenalty,
            MaxTokens = _maxTokens,
            ChoiceCount = _choiceCount,
            PresencePenalty = _presencePenalty,
            ResponseFormat = _responseFormat,
            Stream = _stream,
            Temperature = _temperature,
            TopP = _topP,
            Messages = _messages
        };
    }
}

internal static class CreateChatCompletionRequestDefaults
{
    public static readonly double DefaultFrequencyPenalty = 0.0;
    public static readonly int DefaultMaxTokens = 1000;
    public static readonly int DefaultChoiceCount = 1;
    public static readonly double DefaultPresencePenalty = 0.0;
    public static readonly ResponseFormat DefaultResponseFormat = new()
    {
        Type = "text"
    };
    public static readonly bool DefaultStream = false;
    public static readonly double DefaultTemperature = 1.0;
    public static readonly double DefaultTopP = 1.0;
}
