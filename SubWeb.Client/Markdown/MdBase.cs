using CommonMark;
using System;
using System.Threading.Tasks;

namespace SubWeb.Client.Markdown
{
    public class MdBase
    {
        public async Task<string> ConvertToHtml(string mdFile, Func<string, string> resolveUri)
        {
            CommonMarkSettings settings = null;

            if (resolveUri != null)
            {
                settings = CommonMarkSettings.Default.Clone();
                settings.UriResolver = resolveUri;
            }

            return CommonMarkConverter.Convert(mdFile, settings);
        }
    }
}
