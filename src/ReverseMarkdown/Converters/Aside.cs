using System;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class Aside : ConverterBase
    {
        public Aside(Converter converter)
            : base(converter)
        {
            Converter.Register("aside", this);
        }

        public override string Convert(HtmlNode node)
        {
            return $"{Environment.NewLine}{TreatChildren(node)}{Environment.NewLine}";
        }
    }
}
