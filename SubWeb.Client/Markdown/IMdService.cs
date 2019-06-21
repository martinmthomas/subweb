using SubWeb.Client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubWeb.Client.Markdown
{
    public interface IMdService
    {
        Task<string> DownloadFileAsHtmlAsync(string owner, string repoName, string filePath);
        Task<IEnumerable<NavItem>> GetNavItemsAsync(string owner, string repoName = "", string path = "");
    }
}
