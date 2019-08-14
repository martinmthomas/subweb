namespace SubWeb.Client.Markdown
{
    public class GithubUri
    {
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

        public GithubUri()
        {
            UriParts = new string[0];
        }
    }
}
