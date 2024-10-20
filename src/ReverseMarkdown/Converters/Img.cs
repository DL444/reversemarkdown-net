using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class Img : ConverterBase
    {
        public Img(Converter converter) : base(converter)
        {
            Converter.Register("img", this);
        }

        public override string Convert(HtmlNode node)
        {
            var alt = node.GetAttributeValue("alt", string.Empty);
            var src = node.GetAttributeValue("src", string.Empty);

            if (!Converter.Config.IsSchemeWhitelisted(StringUtils.GetScheme(src)))
            {
                return "";
            }

            var title = ExtractTitle(node);
            title = title.Length > 0 ? $" \"{title}\"" : "";

            return $"![{StringUtils.EscapeLinkText(alt)}]({src}{title})";
        }
    }
}
