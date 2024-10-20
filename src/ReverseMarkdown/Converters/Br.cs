using System;
using System.Linq;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
#if SOURCE_GENERATOR_AVAILABLE
    [SourceGenerator.Converter]
#endif
    public class Br : ConverterBase
    {
        public Br(Converter converter) : base(converter)
        {
            Converter.Register("br", this);
        }

        public override string Convert(HtmlNode node)
        {
            var parentName = node.ParentNode.Name.ToLowerInvariant();
            var parentList = new string[] {"strong", "b", "em", "i"};
            if (parentList.Contains(parentName))
            {
                return "";
            }

            return Converter.Config.GithubFlavored ? Environment.NewLine : $"  {Environment.NewLine}";
        }
    }
}
