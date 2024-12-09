namespace ChatBot.Api.Domain.Exceptions.PromptExceptions;

public sealed class PromptDuplicateKeyException : ApplicationException
{
    public PromptDuplicateKeyException(string key, string owner)
        : base("Prompt with provided key already exists.")
    {
        Data.Add(nameof(key), key);
        Data.Add(nameof(owner), owner);

        Key = key;
        Owner = owner;
    }
    
    public string Key { get; }
    
    public string Owner { get; }
}