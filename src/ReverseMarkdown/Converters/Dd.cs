using System;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class Dd : ConverterBase
    {
        public Dd(Converter converter) : base(converter)
        {
            Converter.Register("dd", this);
        }

        public override string Convert(HtmlNode node)
        {
            var indent = new string(' ', 4);
            var prefix = $"{Converter.Config.ListBulletChar} ";
            var content = TreatChildren(node);
            return $"{indent}{prefix}{content.Chomp()}{Environment.NewLine}";
        }
    }    
}