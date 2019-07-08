### SubWeb

**SubWeb** is an online tool using [Blazor][Blazor] to generate html pages from markdown files hosted in a Github repository. The main driving force behind `SubWeb` is the curiosity to experiment with and understand the capabilities of *Blazor* which is now part of [AspNetCore][AspNetGit]. For this reason, markdown conversion is implemented with the help of [CommonMark.NET][CommonMarkGit] library. 

Because Blazor's client side hosting model is used, `SubWeb` executes completely in browser just like a javascript code. Even the markdown conversion is executed in browser using C#! There were some interesting learnings around Async/Await programming in single threaded browser UI, static hosting, etc. I will be writing a blog to cover these in depth. To see some of the frequently asked questions, see [faq.md][SubWebFAQ].

To see the tool in action, launch [SubWeb][SubWeb].

#### Contributions

All sort of contributions are welcome! If you have a proposed solution to improve some area of the app, then feel free to raise a Pull Request. If you are just curious to know more about the project or have questions, you can reach out to me on [Twitter][Twitter] or [LinkedIn][LinkedIn]


[Blazor]: https://dotnet.microsoft.com/apps/aspnet/web-apps/client
[AspNetGit]: https://github.com/aspnet/AspNetCore
[CommonMarkGit]: https://github.com/Knagis/CommonMark.NET
[SubWebFAQ]: https://subweb.azurewebsites.net/martinmthomas/subweb/faq.md
[SubWeb]: https://subweb.azurewebsites.net
[Twitter]: https://twitter.com/martinmthomas
[LinkedIn]: https://www.linkedin.com/in/martin-mathai-thomas-4bb6a66a/

[link1]: https://github.com/aspnet/Blazor/issues/1413
[link2]: https://itnext.io/mvvm-and-blazor-components-and-statehaschanged-a31be365638b
