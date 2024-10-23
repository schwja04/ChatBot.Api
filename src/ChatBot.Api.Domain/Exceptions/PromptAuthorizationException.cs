namespace ChatBot.Api.Domain.Exceptions;

public class PromptAuthorizationException : ApplicationException
{
	public PromptAuthorizationException(Guid promptId, string unauthorizedUser, params object[] objects)
    {
        base.Data.Add(nameof(promptId), promptId);
        base.Data.Add(nameof(unauthorizedUser), unauthorizedUser);

        for (int i = 0; i < objects.Length; i++)
        {
            base.Data.Add($"extraParams_{i}", objects[i]);
        }
    }

    public override string ToString() => "User is not authorized to access this Prompt";
}

