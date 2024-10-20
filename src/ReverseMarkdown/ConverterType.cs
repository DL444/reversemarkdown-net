using System;
using System.Diagnostics.CodeAnalysis;

namespace ReverseMarkdown
{
    public sealed class ConverterType
    {
        public ConverterType(
#if NET7_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        Type type) => Type = type;
        public Type Type {
#if NET7_0_OR_GREATER
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
            get; }
    }
}
