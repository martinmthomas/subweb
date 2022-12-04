using SubWeb.Client.Ext.Github.Model;

namespace Github.Services
{
    public interface IRepository
    {
        Task<IReadOnlyCollection<ContentInfo>> GetAllContentsAsync(string owner, string repoName, string path);
        Task<RepoFile> GetFileContentAsync(string owner, string repoName, string path);
        Task<List<Repo>> GetAllForUserAsync(string owner);
        Task<SearchResponse> SearchAsync(string owner, DateTimeOffset createdDate, DateTimeOffset lastUpdatedDate, int pageIndex, int itemsPerPage = 10, string sortBy = "stars", string orderBy = "desc");
    }
}