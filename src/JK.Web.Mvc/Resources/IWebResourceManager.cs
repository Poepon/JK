using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace JK.Web.Resources
{
    public interface IWebResourceManager
    {
        void AddScript(string url, bool addMinifiedOnProd = true);

        void AddScriptContent(string content, bool addMinifiedOnProd = true);

        void AddScriptContent(IHtmlContent htmlContent, bool addMinifiedOnProd = true);

        IReadOnlyList<string> GetScripts();

        HelperResult RenderScripts();

        IReadOnlyList<string> GetScriptContents();

        HelperResult RenderScriptContents();
    }
}
