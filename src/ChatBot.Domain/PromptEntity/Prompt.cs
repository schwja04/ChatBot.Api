using ChatBot.Domain.Exceptions.PromptExceptions;

namespace ChatBot.Domain.PromptEntity;

public record Prompt
{
    private Prompt(Guid promptId, string key, string value, Guid ownerId)
    {
        PromptId = promptId;
        Key = key;
        Value = value;
        OwnerId = ownerId;
    }

    public Guid PromptId { get; }

    public string Key { get; private set; }

    public string Value { get; private set; }

    public Guid OwnerId { get; }
    
    public void UpdateValue(string value)
    {
        if (string.Equals(Value, value, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        Value = value;
    }
    
    public void UpdateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new PromptKeyCannotBeEmptyException(PromptId, OwnerId);
        }
        
        if (string.Equals(Key, key, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        Key = key;
    }

    public static Prompt CreateNew(string key, string value, Guid ownerId)
    {
        return new Prompt(
            promptId: Guid.NewGuid(),
            key: key,
            value: value,
            ownerId: ownerId);
    }

    public static Prompt CreateExisting(Guid promptId, string key, string value, Guid ownerId)
    {
        return new Prompt(
            promptId: promptId,
            key: key,
            value: value,
            ownerId: ownerId);
    }
}

public static class PromptKey
{
    public const string Email = nameof(Email);
    public const string None = nameof(None);
    public const string Title = nameof(Title);
}

public static class PromptValue
{
    public const string Email = """
        Format all of your your responses back as an email.

        ### Starting Context Here ###
        {0}
        ### Ending Context Here ###
        """;

    public const string None = "";

    public const string Title = """
        Create a title for our conversation based on the following message.
        Ensure to keep the title to no more than 5 words.
        Do not include special characters.
        Do not use the symbol ".

        ### Starting Context Here ###
        {0}
        ### Ending Context Here ###
        """;
}