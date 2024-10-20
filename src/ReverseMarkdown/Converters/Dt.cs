using System;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class Dt : ConverterBase
    {
        public Dt(Converter converter) : base(converter)
        {
            Converter.Register("dt", this);
        }

        public override string Convert(HtmlNode node)
        {
            var prefix = $"{Converter.Config.ListBulletChar} ";
            var content = TreatChildren(node);
            return $"{prefix}{content.Chomp()}{Environment.NewLine}";
        }
    }    
}