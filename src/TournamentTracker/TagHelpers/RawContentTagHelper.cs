using Microsoft.AspNetCore.Razor.TagHelpers;
using TournamentTracker.Core;

namespace TournamentTracker.TagHelpers;

[HtmlTargetElement("raw")]
public class RawContentTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync(NullHtmlEncoder.Default);
        var html = content.GetContent(NullHtmlEncoder.Default);

        var sanitizedHtml = HtmlContentSanitification.Sanitize(html);
        output.TagName = null;
        output.Content.SetHtmlContent(sanitizedHtml);

        await base.ProcessAsync(context, output);
    }
}