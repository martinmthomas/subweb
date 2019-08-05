using SubWeb.Client.Ext.Github.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Github.Services
{
    public interface IRepository
    {
        Task<IReadOnlyCollection<ContentInfo>> GetAllContents(string owner, string repoName, string path);
        Task<File> GetFileContent(string owner, string repoName, string path);
        Task<List<Repo>> GetAllForUser(string owner);
        Task<SearchResponse> Search(string owner, DateTimeOffset createdDate, DateTimeOffset lastUpdatedDate, int pageIndex, int itemsPerPage = 10, string sortBy = "stars", string orderBy = "desc");
    }
}