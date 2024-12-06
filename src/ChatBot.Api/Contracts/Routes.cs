namespace ChatBot.Api.Contracts;

public static class Routes
{
    private const string ApiRoot = "api";

    public const string Chats = $"{ApiRoot}/chats";
    public const string ChatsByContextId = $"{Chats}/{{contextId}}";
    public const string UpdateChatTitleByContextId = $"{ChatsByContextId}:update-title";

    public const string ChatMetadatas = $"{Chats}/metadatas";

    public const string Prompts = $"{ApiRoot}/prompts";
    public const string PromptsByPromptId = $"{Prompts}/{{promptId}}";
}
