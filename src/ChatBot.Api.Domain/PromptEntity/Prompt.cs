namespace ChatBot.Api.Domain.PromptEntity;

public record Prompt
{
    private Prompt(Guid promptId, string key, string value, string owner)
    {
        PromptId = promptId;
        Key = key;
        Value = value;
        Owner = owner;
    }

    public Guid PromptId { get; }

    public string Key { get; }

    public string Value { get; }

    public string Owner { get; }

    public static Prompt CreateNew(string key, string value, string owner)
    {
        return new Prompt(
            promptId: Guid.NewGuid(),
            key: key,
            value: value,
            owner: owner);
    }

    public static Prompt CreateExisting(Guid promptId, string key, string value, string owner)
    {
        return new Prompt(
            promptId: promptId,
            key: key,
            value: value,
            owner: owner);
    }
}

public static class PromptKey
{
    public static readonly string Email = nameof(Email);
    public static readonly string None = nameof(None);
    public static readonly string Title = nameof(Title);
}

public static class PromptValue
{
    public static readonly string Email = """
        Format all of your your responses back as an email.

        ### Starting Context Here ###
        {0}
        ### Ending Context Here ###
        """;

    public static readonly string None = string.Empty;

    public static readonly string Title = """
        Create a title for our conversation based on the following message.
        Ensure to keep the title to no more than 5 words.
        Do not include special characters.
        Do not use the symbol ".

        ### Starting Context Here ###
        {0}
        ### Ending Context Here ###
        """;
}