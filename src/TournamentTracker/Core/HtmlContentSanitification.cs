using AngleSharp.Css.Dom;
using AngleSharp.Svg.Dom;
using Ganss.Xss;
using TinyHelpers.Extensions;

namespace TournamentTracker.Core;

public static class HtmlContentSanitification
{
    private static readonly HtmlSanitizer htmlSanitizer = new();

    static HtmlContentSanitification()
    {
        htmlSanitizer.AllowedAttributes.Add("class");
        htmlSanitizer.AllowedTags.Add("style");
        htmlSanitizer.AllowedSchemes.Add("data");

        htmlSanitizer.AllowedCssProperties.Add("fill");
        htmlSanitizer.AllowedCssProperties.Add("transform-box");

        htmlSanitizer.AllowedAtRules.Add(CssRuleType.FontFace);
        htmlSanitizer.AllowedAtRules.Add(CssRuleType.Charset);
        htmlSanitizer.AllowedAtRules.Add(CssRuleType.Viewport);
        htmlSanitizer.AllowedAtRules.Add(CssRuleType.Media);
        htmlSanitizer.AllowedAtRules.Add(CssRuleType.Keyframes);
        htmlSanitizer.AllowedAtRules.Add(CssRuleType.Keyframe);
        htmlSanitizer.AllowedAtRules.Add(CssRuleType.Page);

        htmlSanitizer.RemovingTag += new EventHandler<RemovingTagEventArgs>(OnRemovingTag);
        htmlSanitizer.RemovingAttribute += new EventHandler<RemovingAttributeEventArgs>(OnRemovingAttribute);
    }

    public static string Sanitize(string html)
    {
        if (html is null)
        {
            return null;
        }

        var sanitizedHtml = htmlSanitizer.Sanitize(html);
        return sanitizedHtml;
    }

    private static void OnRemovingAttribute(object sender, RemovingAttributeEventArgs handler)
    {
        handler.Cancel = handler.Tag is SvgElement;
    }

    private static void OnRemovingTag(object sender, RemovingTagEventArgs handler)
    {
        if (handler.Tag is SvgElement)
        {
            handler.Cancel = true;
        }
        else
        {
            // Add exceptions for Video IFrame sources.
            if (handler.Tag.NodeName.EqualsIgnoreCase("iframe"))
            {
                var source = handler.Tag.GetAttribute("src");
                if (source.ContainsIgnoreCase("youtube"))
                {
                    handler.Cancel = true;
                }
            }
        }
    }
}