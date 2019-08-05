using Microsoft.AspNetCore.Components;
using SubWeb.Client.Exceptions;
using SubWeb.Client.Ext.Github.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Github.Services
{
    public class Repository : IRepository
    {
        private string GITHUB_CONTENT_URL => $"{UriParts.BASE_URL}/repos/{UriParts.OWNER}/{UriParts.REPO}/contents/{UriParts.PATH}";
        private string GITHUB_REPOS_URL => $"{UriParts.BASE_URL}/users/{UriParts.OWNER}/repos";

        private string GITHUB_REPOS_SEARCH_URL => $"{UriParts.BASE_URL}/search/repositories";

        private HttpClient HttpClient { get; set; }

        public Repository(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }


        public async Task<IReadOnlyCollection<ContentInfo>> GetAllContents(string owner, string repoName, string path)
        {
            Ensure.ArgumentNotEmpty(owner, nameof(owner));
            Ensure.ArgumentNotEmpty(repoName, nameof(repoName));

            var url = GITHUB_CONTENT_URL
                .Replace(UriParts.OWNER, owner)
                .Replace(UriParts.REPO, repoName)
                .Replace(UriParts.PATH, path);

            return (await HttpClient.GetJsonAsync<List<ContentInfo>>(url)).AsReadOnly();
        }


        public async Task<File> GetFileContent(string owner, string repoName, string path)
        {
            Ensure.ArgumentNotEmpty(owner, nameof(owner));
            Ensure.ArgumentNotEmpty(repoName, nameof(repoName));
            Ensure.ArgumentNotEmpty(path, nameof(path));

            var url = GITHUB_CONTENT_URL
                .Replace(UriParts.OWNER, owner)
                .Replace(UriParts.REPO, repoName)
                .Replace(UriParts.PATH, path);

            var repoFileContent = await HttpClient.GetJsonAsync<File>(url);
            repoFileContent.Content = FromBase64String(repoFileContent.Content);
            return repoFileContent;
        }


        public async Task<List<Repo>> GetAllForUser(string owner)
        {
            var url = GITHUB_REPOS_URL.Replace(UriParts.OWNER, owner);

            return await HttpClient.GetJsonAsync<List<Repo>>(url);
        }


        public async Task<SearchResponse> Search(string owner, DateTimeOffset createdDate, DateTimeOffset lastUpdatedDate, int pageIndex, int itemsPerPage = 10, string sortBy = "stars", string orderBy = "desc")
        {
            var query = new[] {
                (Param: "user", Value:owner),
                (Param: "created", Value:$"{createdDate.ToString("yyyy-MM-dd")}..{DateTimeOffset.Now.ToString("yyyy-MM-dd")}"),
                (Param: "pushed", Value: $"{lastUpdatedDate.ToString("yyyy-MM-dd")}..{DateTimeOffset.Now.ToString("yyyy-MM-dd")}")
            }
            .Select(p => $"{p.Param}:{p.Value}")
            .Aggregate((a, b) => $"{a}&{b}");

            var queryString = new[] {
                (Param: "page", Value: pageIndex.ToString()),
                (Param: "per_page", Value: itemsPerPage.ToString()),
                (Param: "order", Value: orderBy),
                (Param: "q", Value: query),
                (Param: "sort", Value: sortBy)
            }
            .Select(p => $"{p.Param}={p.Value}")
            .Aggregate((a, b) => $"{a}&{b}");

            var url = $"{GITHUB_REPOS_SEARCH_URL}?{queryString}";
            return await HttpClient.GetJsonAsync<SearchResponse>(url);
        }

        private string FromBase64String(string encodedText)
        {
            var data = Convert.FromBase64String(encodedText);
            return System.Text.Encoding.ASCII.GetString(data);
        }
    }
}
