using Microsoft.AspNetCore.Razor.TagHelpers;
using TournamentTracker.Core;

namespace TournamentTracker.TagHelpers;

[HtmlTargetElement("raw")]
public class RawContentTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var childContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
        var content = childContent.GetContent(NullHtmlEncoder.Default);

        var html = HtmlContentSanitification.Sanitize(content);
        output.TagName = null;
        output.Content.SetHtmlContent(html);

        await base.ProcessAsync(context, output);
    }
}