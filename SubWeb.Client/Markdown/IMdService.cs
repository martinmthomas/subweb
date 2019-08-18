using SubWeb.Client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubWeb.Client.Markdown
{
    public interface IMdService
    {
        Task<string> DownloadFileAsHtmlAsync(GithubUri githubUri);
        Task<IEnumerable<NavItem>> GetNavItemsAsync(GithubUri githubUri);
        Task<IEnumerable<GitRepo>> GetMostStarredRepos(string user);
    }
}
