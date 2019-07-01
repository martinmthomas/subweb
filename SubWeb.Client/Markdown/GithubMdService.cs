using Octokit;
using SubWeb.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubWeb.Client.Markdown
{
    public class GithubMdService : MdBase, IMdService
    {
        public GitHubClient GitClient => new GitHubClient(new ProductHeaderValue("markdownconv"));
        const string REPO_IDENTIFIER = "subweb-", MARKDOWN_EXT = ".md";
        const string README = "README.md";

        private async Task<(string Owner, string RepoName, string FilePath)> ResolveParamsAsync(string owner, string repoName = "", string filePath = "")
        {
            if (string.IsNullOrWhiteSpace(repoName) && !string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Repository Name cannot be blank when File Path is supplied");

            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner cannot be blank");

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
            var sortedNavItems = CreateNavItems(owner, repoName, files);

            var defaultFile = "";
            if (path.EndsWith(MARKDOWN_EXT))
                defaultFile = owner + "/" + repoName + "/" + path;
            else if (sortedNavItems.Count > 0)
                defaultFile = sortedNavItems.First().Uri;

            if (sortedNavItems.Count > 0)
                sortedNavItems.First(nav => nav.Uri == defaultFile).IsDefault = true;

            return sortedNavItems;
        }

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

        private List<NavItem> CreateNavItems(string owner, string repoName, IReadOnlyList<RepositoryContent> files)
        {
            return files
                .Where(r => (r.Name.EndsWith(MARKDOWN_EXT) || r.Type.Value == ContentType.Dir)
                    && !r.Name.StartsWith("."))
                .Select(r =>
                    new NavItem(
                        r.Name,
                        owner + "/" + repoName + "/" + r.Path,
                        r.Type.Value == ContentType.Dir ? NavType.Directory : NavType.Markdown,
                        false))
                .OrderBy(r => r.Type)
                .ThenBy(r => r.Title)
                .ToList();
        }

        public async Task<string> DownloadFileAsHtmlAsync(string owner, string repoName, string filePath)
        {
            var gitParams = await ResolveParamsAsync(owner, repoName, filePath);
            var mdFile = await DownloadFileAsync(gitParams.Owner, gitParams.RepoName, gitParams.FilePath);
            return await ConvertToHtml(mdFile, ResolveUri);
        }

        private async Task<IReadOnlyList<RepositoryContent>> DownloadFilesAsync(string owner, string repoName, string filePath) =>
            string.IsNullOrWhiteSpace(filePath)
                ? await GitClient.Repository.Content.GetAllContents(owner, repoName)
                : await GitClient.Repository.Content.GetAllContents(owner, repoName, filePath);


        private async Task<string> DownloadFileAsync(string owner, string repoName, string filePath)
        {
            if (!filePath.EndsWith(MARKDOWN_EXT))
                return "";

            var files = await GitClient.Repository.Content.GetAllContents(owner, repoName, filePath);
            return files.First().Content;
        }

        private async Task<string> GetDefaultFilePathAsync(string owner, string repoName)
        {
            var _files = await GitClient.Repository.Content.GetAllContents(owner, repoName);
            return _files.First(f => f.Name.EndsWith(MARKDOWN_EXT)).Path;
        }

        private async Task<string> GetDefaultRepoNameAsync(string owner)
        {
            var repos = await GitClient.Repository.GetAllForUser(owner);
            return repos.First(r => r.Name.StartsWith("subweb-")).Name;
        }

        private string ResolveUri(string uri)
        {
            return $"https://google.com/search?q={uri}";
        }


        public async Task<IEnumerable<GitRepo>> GetMostStarredRepos(string user)
        {
            var req = new SearchRepositoriesRequest()
            {
                User = user,
                SortField = RepoSearchSort.Stars,
                Order = SortDirection.Descending,
                PerPage = 10,
                Created = new DateRange(DateTime.Now.AddYears(-5), DateTime.Now),
                Updated = new DateRange(DateTime.Now.AddDays(-7), DateTime.Now)
            };

            var gitRepos = await GitClient.Search.SearchRepo(req);
            return gitRepos.Items.Select(r => new GitRepo(r.FullName, r.Description, r.StargazersCount));
        }
    }
}
