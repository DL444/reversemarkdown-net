using System;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class Dl : ConverterBase
    {
        public Dl(Converter converter) : base(converter)
        {
            Converter.Register("dl", this);
        }

        public override string Convert(HtmlNode node)
        {
            var prefixSuffix = Environment.NewLine;
            return $"{prefixSuffix}{TreatChildren(node)}{prefixSuffix}";
        }
    }    
}