using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReverseMarkdown.SourceGenerator
{
    internal readonly struct ConverterTypesInfoGenerationTarget : IEquatable<ConverterTypesInfoGenerationTarget>
    {
        public ConverterTypesInfoGenerationTarget(string className, string namespaceName, ImmutableArray<ConverterTypeInfo> converterTypes)
        {
            ClassName = className;
            NamespaceName = namespaceName;
            ConverterTypes = converterTypes;
        }

        public string ClassName { get; }
        public string NamespaceName { get; }
        public ImmutableArray<ConverterTypeInfo> ConverterTypes { get; }

        public bool Equals(ConverterTypesInfoGenerationTarget other)
            => string.Equals(ClassName, other.ClassName, StringComparison.Ordinal)
            && string.Equals(NamespaceName, other.NamespaceName, StringComparison.Ordinal)
            && ConverterTypes.SequenceEqual(other.ConverterTypes);
    }

    internal readonly struct ConverterTypeInfo : IEquatable<ConverterTypeInfo>
    {
        public ConverterTypeInfo(string name, string assemblyName)
        {
            Name = name;
            AssemblyName = assemblyName;
        }

        public string Name { get; }
        public string AssemblyName { get; }

        public bool Equals(ConverterTypeInfo other)
            => string.Equals(Name, other.Name, StringComparison.Ordinal) && string.Equals(AssemblyName, other.AssemblyName, StringComparison.Ordinal);
    }

    internal readonly struct ConverterTypesInfoGenerationTargetInfo : IEquatable<ConverterTypesInfoGenerationTargetInfo>
    {
        public ConverterTypesInfoGenerationTargetInfo(string className, string namespaceName, string assemblyName)
        {
            ClassName = className;
            NamespaceName = namespaceName;
            AssemblyName = assemblyName;
        }

        public string ClassName { get; }
        public string NamespaceName { get; }
        public string AssemblyName { get; }

        public bool Equals(ConverterTypesInfoGenerationTargetInfo other)
            => string.Equals(ClassName, other.ClassName, StringComparison.Ordinal) && string.Equals(NamespaceName, other.NamespaceName, StringComparison.Ordinal) && string.Equals(AssemblyName, other.AssemblyName, StringComparison.Ordinal);
    }
}
