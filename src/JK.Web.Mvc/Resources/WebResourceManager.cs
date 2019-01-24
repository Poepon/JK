using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Timing;
using Microsoft.AspNetCore.Html;
using System.IO;
using System.Text.Encodings.Web;

namespace JK.Web.Resources
{
    public class WebResourceManager : IWebResourceManager
    {
        private readonly IHostingEnvironment _environment;
        private readonly List<string> _scriptUrls;
        private readonly List<string> _scriptContents;

        public WebResourceManager(IHostingEnvironment environment)
        {
            _environment = environment;
            _scriptUrls = new List<string>();
            _scriptContents = new List<string>();
        }

        public void AddScript(string url, bool addMinifiedOnProd = true)
        {
            _scriptUrls.AddIfNotContains(NormalizeUrl(url, "js"));
        }

        public void AddScriptContent(string content, bool addMinifiedOnProd = true)
        {
            _scriptContents.Add(content);
        }

        public void AddScriptContent(IHtmlContent htmlContent, bool addMinifiedOnProd = true)
        {
            var content = RenderHtmlContent(htmlContent);
            _scriptContents.Add(content);
        }

        private string RenderHtmlContent(IHtmlContent htmlContent)
        {
            using (var writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, HtmlEncoder.Default);
                var htmlOutput = writer.ToString();
                return htmlOutput;
            }
        }


        public IReadOnlyList<string> GetScripts()
        {
            return _scriptUrls.ToImmutableList();
        }

        public IReadOnlyList<string> GetScriptContents()
        {
            return _scriptContents.ToImmutableList();
        }

        public HelperResult RenderScripts()
        {
            return new HelperResult(async writer =>
            {
                foreach (var scriptUrl in _scriptUrls)
                {
                    await writer.WriteAsync($"<script src=\"{scriptUrl}?v=" + Clock.Now.Ticks + "\"></script>");
                }
            });
        }

        public HelperResult RenderScriptContents()
        {
            return new HelperResult(async writer =>
            {
                foreach (var scriptContent in _scriptContents)
                {
                    await writer.WriteAsync(scriptContent);
                }
            });
        }

        private string NormalizeUrl(string url, string ext)
        {
            if (_environment.IsDevelopment())
            {
                return url;
            }

            if (url.EndsWith(".min." + ext))
            {
                return url;
            }

            return url.Left(url.Length - ext.Length) + "min." + ext;
        }
    }
}
