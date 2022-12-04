using Github.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SubWeb.Client;
using SubWeb.Client.Markdown;
using SubWeb.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IMdService, GithubMdService>();
builder.Services.AddSingleton<IRepository, Repository>();
builder.Services.AddSingleton<IAlertService, AlertService>();
builder.Services.AddSingleton<IGithubUriService, GithubUriService>();

await builder.Build().RunAsync();
