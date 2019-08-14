using Github.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using SubWeb.Client.Markdown;
using SubWeb.Client.Services;

namespace SubWeb.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMdService, GithubMdService>();
            services.AddSingleton<IRepository, Repository>();
            services.AddSingleton<IAlertService, AlertService>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
