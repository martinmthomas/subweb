using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SubWeb.Client.Markdown;
using SubWeb.Client.Model;
using SubWeb.Client.Services;

namespace SubWeb.Client.Pages.CodeBehind
{
    public class Home : ComponentBase
    {
        public GithubUri githubUri = new GithubUri();

        public string ConvHtml;
        public bool ShowingHtml = false;
        public bool IsLoading = false;


        public IEnumerable<NavItem> NavItems = new NavItem[0];


        public string SampleRepo = "aspnet/aspnetcore";
        public IEnumerable<GitRepo> StarredRepos { get; private set; } = new GitRepo[0];
        public const string DefaultRepoUser = "Microsoft";

        public string GitProjUri = "";

        /// <summary>
        /// We don't need to use the Params. But setting this as a "catch-all" parameter in @page is needed 
        /// to get the route details using NavigationManager.
        /// </summary>
        [Parameter]
        public string? Params { get; set; }

        [Inject]
        public NavigationManager UriHelper { get; set; }

        [Inject]
        public IMdService GithubMdService { get; set; }

        [Inject]
        public IAlertService AlertService { get; set; }

        [Inject]
        public IGithubUriService GithubUriService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;

                if (GithubUriService.IsCurrentUriValid())
                {
                    await LoadPageAsync();
                }
                else
                {
                    await SetHomeContentAsync();
                }
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
            IsLoading = false;
        }

        private async Task SetHomeContentAsync()
        {
            Reset();

            ShowingHtml = false;

            StarredRepos = await GithubMdService.GetMostStarredRepos(DefaultRepoUser);
        }

        public async Task LoadPageAsync()
        {
            Reset();

            if (githubUri.IsMarkdownFile)
            {
                var navTask = GenerateNavItems();
                var bodyTask = GenerateBody();
                await navTask;
                await bodyTask;
            }
            else
            {
                await GenerateNavItems();
                var defaultFile = NavItems.FirstOrDefault(n => n.IsDefault);
                if (defaultFile != null && GithubUri.CheckIfMarkdownFile(defaultFile.Uri))
                {
                    githubUri = GithubUriService.CreateGithubUriModel(defaultFile.Uri);
                    await GenerateBody();
                }
            }
        }

        private void Reset()
        {
            ConvHtml = "";
            NavItems = new NavItem[0];
            githubUri = GithubUriService.CreateGithubUriModel();
        }

        protected override Task OnParametersSetAsync()
        {
            UriHelper.LocationChanged += UriHelperLocationChanged;

            return base.OnParametersSetAsync();
        }

        private async void UriHelperLocationChanged(object? sender, LocationChangedEventArgs args)
        {
            await OnInitializedAsync();

            StateHasChanged();
        }

        private async Task GenerateNavItems()
        {
            try
            {
                var navItems = await GithubMdService.GetNavItemsAsync(githubUri);
                NavItems = navItems != null ? navItems : NavItems;
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        private async Task GenerateBody()
        {
            try
            {
                ConvHtml = await GithubMdService.DownloadFileAsHtmlAsync(githubUri);
                ShowingHtml = true;
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }
        }

        private async Task HandleException(Exception ex)
        {
            Console.WriteLine(ex.StackTrace); //Write to a log destination in server? Not for now, as the aim is to do Client side hosting only.

            await AlertService.ErrorAsync(ex.Message);

            if (StarredRepos.Count() == 0)
                await SetHomeContentAsync();
        }
    }
}
