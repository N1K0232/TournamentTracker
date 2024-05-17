using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TournamentTracker.TagHelpers;

[HtmlTargetElement(Attributes = AttributeName)]
public class IsVisibleTagHelper : TagHelper
{
    public const string AttributeName = "is-visible";

    public bool IsVisible { get; set; } = true;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (!IsVisible)
        {
            output.SuppressOutput();
        }

        await base.ProcessAsync(context, output);
    }
}