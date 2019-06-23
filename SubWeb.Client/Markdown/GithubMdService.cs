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

            // if the path points to a markdown file, then find its folder 
            var folder = "";
            if (!string.IsNullOrWhiteSpace(path))
                if (path.EndsWith(MARKDOWN_EXT))
                    if (path.Contains("/"))
                        folder = path.Substring(0, path.LastIndexOf("/"));
                    else
                        folder = "";
                else
                    folder = path;

            var files = string.IsNullOrWhiteSpace(folder)
                ? await GitClient.Repository.Content.GetAllContents(owner, repoName)
                : await GitClient.Repository.Content.GetAllContents(owner, repoName, folder);


            var sortedNavItems = files
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


            var defaultFile = "";
            if (path.EndsWith(MARKDOWN_EXT))
                defaultFile = owner + "/" + repoName + "/" + path;
            else if (sortedNavItems.Count > 0)
                defaultFile = sortedNavItems.First().Uri;

            if (sortedNavItems.Count > 0)
                sortedNavItems.First(nav => nav.Uri == defaultFile).IsDefault = true;

            return sortedNavItems;
        }

        public async Task<string> DownloadFileAsHtmlAsync(string owner, string repoName, string filePath)
        {
            var gitParams = await ResolveParamsAsync(owner, repoName, filePath);
            var mdFile = await DownloadFileAsync(gitParams.Owner, gitParams.RepoName, gitParams.FilePath);
            return await ConvertToHtml(mdFile, ResolveUri);
        }


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
    }
}
