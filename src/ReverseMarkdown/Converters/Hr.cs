using System;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class Hr : ConverterBase
    {
        public Hr(Converter converter) : base(converter)
        {
            Converter.Register("hr", this);
        }

        public override string Convert(HtmlNode node)
        {
            return $"{Environment.NewLine}* * *{Environment.NewLine}";
        }
    }
}
