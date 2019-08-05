using SubWeb.Client.Exceptions;
using SubWeb.Client.Ext.Github.Model;
using SubWeb.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubWeb.Client.Markdown
{
    public class GithubMdService : MdBase, IMdService
    {
        const string REPO_IDENTIFIER = "subweb-", MARKDOWN_EXT = ".md";
        const string README = "README.md";

        private Github.Services.IRepository GithubRepo { get; set; }

        public GithubMdService(Github.Services.IRepository githubRepo)
        {
            GithubRepo = githubRepo;
        }

        private async Task<(string Owner, string RepoName, string FilePath)> ResolveParamsAsync(string owner, string repoName = "", string filePath = "")
        {
            if (!string.IsNullOrWhiteSpace(filePath))
                Ensure.ArgumentNotEmpty(repoName, nameof(repoName));

            Ensure.ArgumentNotEmpty(owner, nameof(owner));

            repoName = string.IsNullOrWhiteSpace(repoName) ? await GetDefaultRepoNameAsync(owner) : repoName;
            filePath = string.IsNullOrWhiteSpace(filePath) ? await GetDefaultFilePathAsync(owner, repoName) : filePath;

            return (owner, repoName, filePath);
        }


        public async Task<IEnumerable<NavItem>> GetNavItemsAsync(string owner, string repoName = "", string path = "")
        {
            repoName = string.IsNullOrWhiteSpace(repoName)
                ? await GetDefaultRepoNameAsync(owner)
                : repoName;

            var fileContainer = GetContainerName(path);
            var files = await DownloadFilesAsync(owner, repoName, fileContainer);

            var sortedNavItems = CreateNavItems(owner, repoName, path, files);

            return sortedNavItems;
        }

        public bool IsMarkdownFile(string path) => path.EndsWith(MARKDOWN_EXT);



        private string GetContainerName(string path)
        {
            // NOTE: aspnet/AspNetCore is taken as the example repository and project in the below examples mentioned in comments.
            var containerName = "";

            if (!string.IsNullOrWhiteSpace(path))
                if (path.EndsWith(MARKDOWN_EXT))
                    if (path.Contains("/")) //eg: docs/Artifacts.md. Note that "docs" is the container here.
                        containerName = path.Substring(0, path.LastIndexOf("/"));
                    else // eg: README.md. Note the there is no container here.
                        containerName = "";
                else //eg: docs. Note that this already points to a container
                    containerName = path;

            return containerName;
        }

        private List<NavItem> CreateNavItems(string owner, string repoName, string requestPath, IReadOnlyCollection<ContentInfo> files)
        {
            var navItems = files
                .Where(r => (r.Name.EndsWith(MARKDOWN_EXT) || r.Type == "dir")
                    && !r.Name.StartsWith("."))
                .Select(r =>
                    new NavItem(
                        r.Name,
                        owner + "/" + repoName + "/" + r.Path,
                        r.Type == "dir" ? NavType.Directory : NavType.Markdown,
                        false))
                .OrderBy(r => r.Type)
                .ThenBy(r => r.Title)
                .ToList();

            if (navItems != null && navItems.Count > 0)
            {
                if (IsMarkdownFile(requestPath))
                    navItems.First(n => n.Uri.EndsWith(requestPath)).IsDefault = true;
                else if (navItems.Any(n => n.Uri.EndsWith(README)))
                    navItems.First(n => n.Uri.EndsWith(README)).IsDefault = true;
                else if (navItems.Any(n => IsMarkdownFile(n.Uri)))
                    navItems.First(n => IsMarkdownFile(n.Uri)).IsDefault = true;
            }

            return navItems;
        }

        public async Task<string> DownloadFileAsHtmlAsync(string owner, string repoName, string filePath)
        {
            var gitParams = await ResolveParamsAsync(owner, repoName, filePath);
            var mdFile = await DownloadFileAsync(gitParams.Owner, gitParams.RepoName, gitParams.FilePath);
            return await ConvertToHtml(mdFile, ResolveUri);
        }

        private async Task<IReadOnlyCollection<ContentInfo>> DownloadFilesAsync(string owner, string repoName, string filePath) =>
            string.IsNullOrWhiteSpace(filePath)
                ? await GithubRepo.GetAllContents(owner, repoName, "")
                : await GithubRepo.GetAllContents(owner, repoName, filePath);


        private async Task<string> DownloadFileAsync(string owner, string repoName, string filePath)
        {
            if (!IsMarkdownFile(filePath))
                return "";

            var file = await GithubRepo.GetFileContent(owner, repoName, filePath);
            return file.Content;
        }

        private async Task<string> GetDefaultFilePathAsync(string owner, string repoName)
        {
            var _files = await GithubRepo.GetAllContents(owner, repoName, "");
            return _files.First(f => f.Name.EndsWith(MARKDOWN_EXT)).Path;
        }

        private async Task<string> GetDefaultRepoNameAsync(string owner)
        {
            var repos = await GithubRepo.GetAllForUser(owner);
            return repos.First(r => r.Name.StartsWith(REPO_IDENTIFIER)).Name;
        }

        private string ResolveUri(string uri)
        {
            return uri;
        }


        public async Task<IEnumerable<GitRepo>> GetMostStarredRepos(string user)
        {
            var gitRepos = await GithubRepo.Search(user, DateTimeOffset.Now.AddYears(-5), DateTimeOffset.Now.AddDays(-7), 1, 10, "stars", "desc");

            return gitRepos.Items.Select(r => new GitRepo(r.Full_Name, r.Description, r.Stargazers_Count));
        }
    }
}
