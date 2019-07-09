SubWeb
=======

`SubWeb` is an online tool created using [Blazor][Blazor] to generate html pages from markdown files hosted in a Github repository. The main driving force behind SubWeb is the curiosity to experiment with and understand the capabilities of Blazor which is now part of [AspNetCore][AspNetGit]. For this reason, markdown conversion is implemented with the help of [CommonMark.NET][CommonMarkGit] library. 

Because Blazor's Client-side hosting model is used, SubWeb executes completely in browser just like a javascript code. Even the markdown conversion is executed in browser using C#! There were some interesting learnings around Async/Await programming in single threaded browser UI, static hosting, etc. I will be writing a blog to cover these in depth. To see some of the frequently asked questions, see [faq.md][SubWebFAQ].

## Get Started

SubWeb parses the Url to infer the github repository name, project name and markdown path. For example, when a user navigates to https://subweb.azurewebsites.net/aspnet/AspNetCore, SubWeb looks under *AspNetCore* project in *aspnet* repository in Github. SubWeb will display all the available markdown files that are present in the root path of the project in a sidebar and will also open README.md file by default, if there is one present. If the Url includes a markdown file path, then that file will be opened by default instead of a README.md file.

To see the tool in action, launch [SubWeb][SubWeb], enter a github project path like *aspnet/AspNetCore* in textbox and press Load. If you are comfortable typing the Url directly, you could enter the Url in address bar in the format,
https://subweb.azurewebsites.net/{GithubRepository}/{GithubProjectName}/{MarkdownFilePath:Optional}

**Note:**

1. Markdown file path mentioned above refers to the relative file path and not the Github Url. For example, README.md file in AspNetCore project is accessible through the Url, https://github.com/aspnet/AspNetCore/blob/master/README.md. However, the relative path of the file to be used with SubWeb Url is *aspnet/AspNetCore/README.md*.
2. Urls are case sensitive. So if you want to access the README.md file mentioned before, then the Url to be used must be https://subweb.azurewebsites.net/aspnet/AspNetCore/README.md.

## Contributions

If you have a proposed solution to improve some area of the app, then feel free to raise a Pull Request. If you are just curious to know more about the project or have questions, you can reach out to me on [Twitter][Twitter] or on [LinkedIn][LinkedIn]


[Blazor]: https://dotnet.microsoft.com/apps/aspnet/web-apps/client
[AspNetGit]: https://github.com/aspnet/AspNetCore
[CommonMarkGit]: https://github.com/Knagis/CommonMark.NET
[SubWebFAQ]: https://subweb.azurewebsites.net/martinmthomas/subweb/faq.md
[SubWeb]: https://subweb.azurewebsites.net
[Twitter]: https://twitter.com/martinmthomas
[LinkedIn]: https://www.linkedin.com/in/martin-mathai-thomas-4bb6a66a/

[link1]: https://github.com/aspnet/Blazor/issues/1413
[link2]: https://itnext.io/mvvm-and-blazor-components-and-statehaschanged-a31be365638b
