using SubWeb.Client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubWeb.Client.Markdown
{
    public interface IMdService
    {
        Task<string> DownloadFileAsHtmlAsync(string owner, string repoName, string filePath);
        bool DoesPathReferToMarkdownFile(string path);
        Task<IEnumerable<NavItem>> GetNavItemsAsync(string owner, string repoName = "", string path = "");
        Task<IEnumerable<GitRepo>> GetMostStarredRepos(string user);
    }
}
