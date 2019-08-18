using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubWeb.Client.Markdown
{
    public class GithubUriService : IGithubUriService
    {
        private IUriHelper _uriHelper;

        private string[] DoubleSlash => new string[] { "//" };
        private string[] SingleSlash => new string[] { "/" };


        public GithubUriService(IUriHelper uriHelper)
        {
            _uriHelper = uriHelper;
        }


        private string[] GetUriParts(string uri) =>
             uri.Split(DoubleSlash, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(SingleSlash, StringSplitOptions.RemoveEmptyEntries);

        public bool IsCurrentUriValid() => GetUriParts(_uriHelper.GetAbsoluteUri()).Length > 1;

        public GithubUri CreateGithubUriModel() => new GithubUri(GetUriParts(_uriHelper.GetAbsoluteUri()));

        public GithubUri CreateGithubUriModel(string uri)
        {
            uri = uri.StartsWith(_uriHelper.GetBaseUri()) ? uri : (_uriHelper.GetBaseUri() + uri);
            return new GithubUri(GetUriParts(uri));
        }
    }
}
