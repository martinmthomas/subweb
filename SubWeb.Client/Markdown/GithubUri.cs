namespace SubWeb.Client.Markdown
{
    public class GithubUri
    {
        public const string MARKDOWN_EXT = ".md";

        public string[] UriParts { get; set; }
        public string Owner => UriParts.Length > 1 ? UriParts[1] : "";
        public string RepoName => UriParts.Length > 2 ? UriParts[2] : "";
        public string FilePath
        {
            get
            {
                if (UriParts.Length > 3)
                {
                    var path = "";
                    for (int i = 3; i < UriParts.Length; i++)
                        path += UriParts[i] + "/";
                    return path.TrimEnd('/');
                }

                return "";
            }
        }

        public bool IsMarkdownFile => CheckIfMarkdownFile(FilePath);

        public static bool CheckIfMarkdownFile(string path) => path.EndsWith(MARKDOWN_EXT);


        public string ContainerPath
        {
            get
            {
                // NOTE: aspnet/AspNetCore is taken as the example repository and project in the below examples mentioned in comments.
                var containerPath = "";

                if (!string.IsNullOrWhiteSpace(FilePath))
                    if (IsMarkdownFile)
                        if (FilePath.Contains("/")) //eg: docs/Artifacts.md. Note that "docs" is the container here.
                            containerPath = FilePath.Substring(0, FilePath.LastIndexOf("/"));
                        else // eg: README.md. Note the there is no container here.
                            containerPath = "";
                    else //eg: docs. Note that this already points to a container
                        containerPath = FilePath;

                return containerPath;
            }
        }

        public GithubUri(string[] uriParts)
        {
            UriParts = uriParts;
        }

        public GithubUri()
        {
            UriParts = new string[0];
        }
    }
}
