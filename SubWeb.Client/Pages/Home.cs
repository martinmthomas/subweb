using Microsoft.AspNetCore.Components;
using SubWeb.Client.Markdown;
using SubWeb.Client.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubWeb.Client.Pages.CodeBehind
{
    public class Home : ComponentBase
    {
        private string[] UriParts;
        public string GitOwner => UriParts.Length > 1 ? UriParts[1] : "";
        public string GitRepoName => UriParts.Length > 2 ? UriParts[2] : "";
        public string GitFilePath
        {
            get
            {
                if (UriParts.Length > 3)
                {
                    var path = "";
                    for (int i = 3; i < UriParts.Length; i++)
                        path += UriParts[i] + "/";
                    return path.TrimEnd('/');
                }

                return "";
            }
        }

        public string ConvHtml;
        public string ExceptionMessage;

        public IEnumerable<NavItem> NavItems = new NavItem[0];

        [Inject]
        public IUriHelper UriHelper { get; set; }

        [Inject]
        public IMdService GithubMdService { get; set; }



        protected override async Task OnInitAsync()
        {
            var uri = UriHelper.GetAbsoluteUri();
            if (IsValidUrl(uri))
            {
                await LoadPageAsync(uri);
            }
            else
            {
                ExceptionMessage = "Invalid Url";
            }

            UriHelper.OnLocationChanged += UriHelper_OnLocationChanged;
        }

        private async void UriHelper_OnLocationChanged(object sender, string e)
        {
            ExceptionMessage = e;
            await LoadPageAsync(e);
        }

        public async Task LoadPageAsync(string uri)
        {
            SetUriPartsForGit(uri);
            await GenerateNavItems();
            await GenerateBody();
        }


        public async Task NavigateToAsync(string uri)
        {
            UriHelper.NavigateTo(uri);
            await LoadPageAsync(uri);
        }

        private async Task GenerateNavItems()
        {
            try
            {
                var navItems = await GithubMdService.GetNavItemsAsync(GitOwner, GitRepoName, GitFilePath);
                NavItems = navItems != null ? navItems : NavItems;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void SetUriPartsForGit(string uri)
        {
            uri = uri.StartsWith(UriHelper.GetBaseUri()) ? uri : (UriHelper.GetBaseUri() + uri);
            UriParts = uri
                .Split(new string[] { "//" }, StringSplitOptions.None)[1]
                .Split('/');
        }

        private bool IsValidUrl(string uri) => (uri.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
                .Length) > 1;

        private async Task GenerateBody()
        {
            try
            {
                ConvHtml = await GithubMdService.DownloadFileAsHtmlAsync(GitOwner, GitRepoName, GitFilePath);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void HandleException(Exception ex)
        {
            ExceptionMessage = ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace;
        }
    }
}
