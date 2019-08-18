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
        const string REPO_IDENTIFIER = "subweb-";
        const string README = "README.md";

        private Github.Services.IRepository _githubRepo;

        public GithubMdService(Github.Services.IRepository githubRepo)
        {
            _githubRepo = githubRepo;
        }


        public async Task<IEnumerable<NavItem>> GetNavItemsAsync(GithubUri githubUri)
        {
            var files = await DownloadFilesAsync(githubUri);

            return CreateNavItems(githubUri, files);
        }


        public async Task<string> DownloadFileAsHtmlAsync(GithubUri githubUri)
        {
            var gitParams = await ResolveParamsAsync(githubUri);
            var mdFile = await DownloadMarkdownFileAsync(gitParams.Owner, gitParams.RepoName, gitParams.FilePath);
            return await ConvertToHtml(mdFile, ResolveUri);
        }


        public async Task<IEnumerable<GitRepo>> GetMostStarredRepos(string user)
        {
            var gitRepos = await _githubRepo.SearchAsync(user, DateTimeOffset.Now.AddYears(-5), DateTimeOffset.Now.AddDays(-7), 1, 10, "stars", "desc");

            return gitRepos.Items.Select(r => new GitRepo(r.Full_Name, r.Description, r.Stargazers_Count));
        }



        private async Task<IReadOnlyCollection<ContentInfo>> DownloadFilesAsync(GithubUri githubUri)
        {
            var repoName = await GetRepoNameOrDefaultAsync(githubUri);
            return await _githubRepo.GetAllContentsAsync(githubUri.Owner, repoName, githubUri.ContainerPath);
        }


        private List<NavItem> CreateNavItems(GithubUri githubUri, IReadOnlyCollection<ContentInfo> files)
        {
            var navItems = files
                .Where(r => (GithubUri.CheckIfMarkdownFile(r.Name) || r.Type == "dir")
                    && !r.Name.StartsWith("."))
                .Select(r =>
                    new NavItem(
                        r.Name,
                        githubUri.Owner + "/" + githubUri.RepoName + "/" + r.Path,
                        r.Type == "dir" ? NavType.Directory : NavType.Markdown,
                        false))
                .OrderBy(r => r.Type)
                .ThenBy(r => r.Title)
                .ToList();

            if (navItems != null && navItems.Count > 0)
            {
                if (githubUri.IsMarkdownFile)
                    navItems.First(n => n.Uri.EndsWith(githubUri.FilePath)).IsDefault = true;
                else if (navItems.Any(n => n.Uri.EndsWith(README)))
                    navItems.First(n => n.Uri.EndsWith(README)).IsDefault = true;
                else if (navItems.Any(n => GithubUri.CheckIfMarkdownFile(n.Uri)))
                    navItems.First(n => GithubUri.CheckIfMarkdownFile(n.Uri)).IsDefault = true;
            }

            return navItems;
        }


        private async Task<string> DownloadMarkdownFileAsync(string owner, string repoName, string filePath)
        {
            if (!GithubUri.CheckIfMarkdownFile(filePath))
                return "";

            var file = await _githubRepo.GetFileContentAsync(owner, repoName, filePath);
            return file.Content;
        }


        private async Task<(string Owner, string RepoName, string FilePath)> ResolveParamsAsync(GithubUri githubUri)
        {
            if (!string.IsNullOrWhiteSpace(githubUri.FilePath))
                Ensure.ArgumentNotEmpty(githubUri.RepoName, nameof(githubUri.RepoName));

            Ensure.ArgumentNotEmpty(githubUri.Owner, nameof(githubUri.Owner));

            var repoName = await GetRepoNameOrDefaultAsync(githubUri);
            var filePath = await GetFilePathOrDefaultAsync(githubUri);

            return (githubUri.Owner, repoName, filePath);
        }

        private async Task<string> GetFilePathOrDefaultAsync(GithubUri githubUri)
        {
            if (!string.IsNullOrWhiteSpace(githubUri.FilePath))
                return githubUri.FilePath;

            var _files = await _githubRepo.GetAllContentsAsync(githubUri.Owner, githubUri.RepoName, "");
            return _files.First(f => GithubUri.CheckIfMarkdownFile(f.Name)).Path;
        }

        private async Task<string> GetRepoNameOrDefaultAsync(GithubUri githubUri)
        {
            if (!string.IsNullOrWhiteSpace(githubUri.RepoName))
                return githubUri.RepoName;

            var repos = await _githubRepo.GetAllForUserAsync(githubUri.Owner);
            return repos.First(r => r.Name.StartsWith(REPO_IDENTIFIER)).Name;
        }

        private string ResolveUri(string uri)
        {
            return uri;
        }
    }
}
