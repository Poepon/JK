using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace JK.Web.CdnTagHelpers
{
    [HtmlTargetElement("head")]
    public class PreConnectTagHelper : TagHelper
    {
        private string _cdnUrl;
        private bool _dnsPrefetch;

        public PreConnectTagHelper(IOptionsMonitor<CdnOptions> options)
        {
            _cdnUrl = options.CurrentValue.Url;
            _dnsPrefetch = options.CurrentValue.Prefetch;
        }
     

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_dnsPrefetch  || // opted out manually
                string.IsNullOrWhiteSpace(_cdnUrl) ||
                string.IsNullOrEmpty(output.TagName))
            {
                return;
            }

            var url = new Uri(_cdnUrl, UriKind.Absolute);
            var link = new HtmlString($"<link rel=\"preconnect\" href=\"{url.OriginalString}\" />");

            output.PreContent.AppendHtml(link);
        }
    }
}
