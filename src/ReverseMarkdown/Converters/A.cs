﻿using HtmlAgilityPack;
using System;
using System.Linq;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class A : ConverterBase
    {
        public A(Converter converter)
            : base(converter)
        {
            Converter.Register("a", this);
        }

        public override string Convert(HtmlNode node)
        {
            var name = TreatChildren(node).Trim();

            var hasSingleChildImgNode = node.ChildNodes.Count == 1 && node.ChildNodes.Count(n => n.Name.Contains("img")) == 1;

            var href = node.GetAttributeValue("href", string.Empty).Trim().Replace("(", "%28").Replace(")", "%29").Replace(" ", "%20");
            var title = ExtractTitle(node);
            title = title.Length > 0 ? $" \"{title}\"" : "";
            var scheme = StringUtils.GetScheme(href);

            var isRemoveLinkWhenSameName = Converter.Config.SmartHrefHandling
                                           && scheme != string.Empty
                                           && Uri.IsWellFormedUriString(href, UriKind.RelativeOrAbsolute)
                                           && (
                                               href.Equals(name, StringComparison.OrdinalIgnoreCase)
                                               || href.Equals($"tel:{name}", StringComparison.OrdinalIgnoreCase)
                                               || href.Equals($"mailto:{name}", StringComparison.OrdinalIgnoreCase)
                                           );

            if (href.StartsWith("#") //anchor link
                || !Converter.Config.IsSchemeWhitelisted(scheme) //Not allowed scheme
                || isRemoveLinkWhenSameName
                || string.IsNullOrEmpty(href)) //We would otherwise print empty () here...
            {
                return name;
            }
            
            // if (!string.IsNullOrEmpty(href) && string.IsNullOrEmpty(name))
            // {
            //     name = href;
            // }

            var useHrefWithHttpWhenNameHasNoScheme = Converter.Config.SmartHrefHandling &&
                                                     (scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                                                     && string.Equals(href, $"{scheme}://{name}", StringComparison.OrdinalIgnoreCase);

            // if the anchor tag contains a single child image node don't escape the link text
            var linkText = hasSingleChildImgNode ? name : StringUtils.EscapeLinkText(name);

            if (string.IsNullOrEmpty(linkText))
            {
                return href;
            }

            return useHrefWithHttpWhenNameHasNoScheme ? href : $"[{linkText}]({href}{title})";
        }
    }
}
