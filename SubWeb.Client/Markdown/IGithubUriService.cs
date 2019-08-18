namespace SubWeb.Client.Markdown
{
    public interface IGithubUriService
    {
        bool IsCurrentUriValid();
        GithubUri CreateGithubUriModel();
        GithubUri CreateGithubUriModel(string uri);
    }
}