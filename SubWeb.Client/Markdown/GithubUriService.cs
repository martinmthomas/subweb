using Microsoft.AspNetCore.Components;

namespace SubWeb.Client.Markdown
{
    public class GithubUriService : IGithubUriService
    {
        private NavigationManager _uriHelper;

        private string[] DoubleSlash => new string[] { "//" };
        private string[] SingleSlash => new string[] { "/" };


        public GithubUriService(NavigationManager uriHelper)
        {
            _uriHelper = uriHelper;
        }


        private string[] GetUriParts(string uri) =>
             uri.Split(DoubleSlash, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(SingleSlash, StringSplitOptions.RemoveEmptyEntries);

        public bool IsCurrentUriValid() => GetUriParts(_uriHelper.Uri).Length > 1;

        public GithubUri CreateGithubUriModel() => new GithubUri(GetUriParts(_uriHelper.Uri));

        public GithubUri CreateGithubUriModel(string uri)
        {
            uri = uri.StartsWith(_uriHelper.BaseUri) ? uri : (_uriHelper.BaseUri + uri);
            return new GithubUri(GetUriParts(uri));
        }
    }
}
