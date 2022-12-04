SubWeb
=======

`SubWeb` is an online tool created using [Blazor][Blazor] to generate html pages from markdown files 
hosted in a Github repository. Markdown conversion is implemented using [CommonMark.NET][CommonMarkGit] library. 

Because Blazor's Client-side hosting model is used, SubWeb executes completely in browser just like a javascript code. 
Even the markdown conversion is executed in browser using C#!

## Getting Started

SubWeb parses the Url to find the github repository name, project name and markdown path. For example, 
when a user navigates to https://subweb.azurewebsites.net/aspnet/AspNetCore, SubWeb looks under *AspNetCore* project 
in *aspnet* repository in Github. SubWeb will display all the available markdown files that are present 
in the root path of the project in a sidebar and will also open README.md file by default, if there is one present. 
If the Url includes a markdown file path, then that file will be opened by default instead of a README.md file.

To see the tool in action, launch [SubWeb][SubWeb], enter a github project path like *aspnet/aspnetcore* in textbox 
and press Load. If you are comfortable typing the Url directly, you could enter the Url in address bar in the format,
https://subweb.azurewebsites.net/{GithubRepository}/{GithubProjectName}/{MarkdownFilePath:Optional}

**Note:**

1. Markdown file path mentioned above refers to the relative file path and not the Github Url. 
   For example, README.md file in AspNetCore project is accessible through the Url, 
   https://github.com/aspnet/AspNetCore/blob/master/README.md. However, the relative path of the file to be used 
   with SubWeb Url is *aspnet/AspNetCore/README.md*.

## WishList
1. Refactor Home page and add Unit Tests
2. Provide support to load images and other assets saved in github as part of the converted html page
3. Add authentication to avoid Github rate limiting
4. User registration and support "DNS to github project" mapping
5. Provide support for custom styling of the converted web page

[Blazor]: https://dotnet.microsoft.com/apps/aspnet/web-apps/client
[CommonMarkGit]: https://github.com/Knagis/CommonMark.NET
[SubWeb]: https://subweb.azurewebsites.net
