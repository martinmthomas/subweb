using Microsoft.AspNetCore.Components;
using SubWeb.Client.Markdown;
using SubWeb.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool ShowingHtml = false;
        public string ExceptionMessage;


        public bool collapseNavMenu = true;
        public string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        public IEnumerable<NavItem> NavItems = new NavItem[0];


        private string SampleRepo = "aspnet/aspnetcore";
        public string HomeContent = "Enter a github project url in the format {BaseUrl}/{git.username}/{git.repository} to load the markdown pages. " +
            "For example, to load https://github.com/{SampleRepo} repository, enter url as " +
            "<span class=\"alert-link\">{BaseUrl}/{SampleRepo}</span>";

        public IEnumerable<GitRepo> StarredRepos { get; private set; } = new GitRepo[0];
        public const string DefaultRepoUser = "Microsoft";


        [Inject]
        public IUriHelper UriHelper { get; set; }

        [Inject]
        public IMdService GithubMdService { get; set; }


        public void ToggleNavMenu() => collapseNavMenu = !collapseNavMenu;

        protected override async Task OnInitAsync()
        {

            try
            {
                var uri = UriHelper.GetAbsoluteUri();
                if (IsValidUrl(uri))
                {
                    await LoadPageAsync(uri);
                }
                else
                {
                    await SetHomeContentAsync();
                }
            }
            catch(Exception ex)
            {
                HandleException(ex);
            }
        }

        private async Task SetHomeContentAsync()
        {
            NavItems = new NavItem[0];
            ConvHtml = "";
            ShowingHtml = false;

            HomeContent = HomeContent
                .Replace("{BaseUrl}", UriHelper.GetBaseUri().TrimEnd('/'))
                .Replace("{SampleRepo}", SampleRepo);

            StarredRepos = await GithubMdService.GetMostStarredRepos(DefaultRepoUser);
        }

        public async Task LoadPageAsync(string uri)
        {
            Reset();

            SetUriPartsForGit(uri);

            if (GithubMdService.IsMarkdownFile(GitFilePath))
            {
                var navTask = GenerateNavItems();
                var bodyTask = GenerateBody();
                await navTask;
                await bodyTask;
            }
            else
            {
                await GenerateNavItems();
                var defFile = NavItems.FirstOrDefault(n => n.IsDefault);
                if (defFile != null && GithubMdService.IsMarkdownFile(defFile.Uri))
                {
                    SetUriPartsForGit(defFile.Uri);
                    await GenerateBody();
                }
            }
        }

        private void Reset()
        {
            ExceptionMessage = "";
            ConvHtml = "";
        }

        protected override Task OnParametersSetAsync()
        {
            UriHelper.OnLocationChanged += UriHelper_OnLocationChanged;

            return base.OnParametersSetAsync();
        }

        private async void UriHelper_OnLocationChanged(object sender, string e)
        {
            await OnInitAsync();

            this.StateHasChanged();
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
                ShowingHtml = true;
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
