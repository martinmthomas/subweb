### Frequently Asked Questions

**1. What is Blazor?**

*Blazor lets you build interactive web UIs using C# instead of JavaScript. Blazor apps are composed of reusable web UI components implemented using C#, HTML, and CSS. Blazor can run your client-side C# code directly in the browser, using WebAssembly. Because it's real .NET running on WebAssembly, you can re-use code and libraries from server-side parts of your application. In short, Blazor lets you write web applications without requiring to write a single line of javascript.*

**2. Are there really no javascript?**
*No, SubWeb has not got a sinle line of javascript! All the client side logic, like generating Navigation items, Markdown conversion, etc. are all written in pure C# and as a Single Page Application. However, behind the scenes Blazor uses javascript to achieve some of its functionalities like Url routing. But this is oblivious to normal developer. Also, it is important to note that Blazor also provides javascript interop if one needs to write javacsript along with C#*

**3. Why first time loading is slow?**

*In order to make C# code executable in browser, Blazor needs to download the core libraries to provide the .net ecosystem. Currently, there are multiple of them and these take around 3MB space in total. Downloading these files to browser could take a while depending on the connection speed. Note that this should be a one time issue only as Blazor caches the libraries just like any other asset.*

**4. Sometimes I get errors that url is incorrect**

*Urls are case sensitive. Most of the DNS servers and web servers tries to guess the right web page by trying to resolve the letter case by itself. But this is not a comprehensive solution. This is a less known fact about internet. So if a file is saved as faq.md in Github, then the url must end with "faq.md" and not FAQ.md or Faq.md.*

**5. What's next for SubWeb?**

*There is a lot that could be improved in SubWeb. Current wish list includes,*
- *Refactor Home page and add Unit Tests :)*
- *Replace OctoKit with Rest Api calls to improve page load performance*
- *Provide support to load images and other assets saved in github as part of the converted html page*
- *Add authentication to avoid Github rate limiting*
- *User registration and support DNS to github project mapping*
- *Provide support for custom styling of the converted web page*
