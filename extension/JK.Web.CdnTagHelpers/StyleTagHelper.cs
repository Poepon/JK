using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace JK.Web.CdnTagHelpers
{
    [HtmlTargetElement("style")]
    [HtmlTargetElement("link", Attributes = "inline")] // Support for LigerShark.WebOptimizer
    public class StyleTagHelper : TagHelper
    {
        private string _cdnUrl;
        private static readonly Regex _rxUrl = new Regex(@"url\s*\(\s*([""']?)([^:)]+)\1\s*\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public StyleTagHelper(IOptionsMonitor<CdnOptions> options)
        {
            _cdnUrl = options.CurrentValue.Url;
        }

        public override int Order => base.Order + 100;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(_cdnUrl) || output.TagName != "style")
            {
                return;
            }

            string css = output.Content.GetContent();

            IEnumerable<Match> matches = _rxUrl.Matches(css).Cast<Match>().Reverse();

            foreach (Match match in matches)
            {
                Group group = match.Groups[2];
                string value = group.Value;

                // Ignore references with protocols
                if (value.Contains("://") || value.StartsWith("//") || value.StartsWith("data:"))
                    continue;

                string sep = value.StartsWith("/") ? "" : "/";

                css = css.Insert(group.Index, $"{_cdnUrl.TrimEnd('/')}{sep}");
            }

            output.Content.SetHtmlContent(css);
        }
    }
}
