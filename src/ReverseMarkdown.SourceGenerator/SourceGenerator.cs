using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace ReverseMarkdown.SourceGenerator
{
    [Generator]
    internal class SourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource("ReverseMarkdownSourceGeneratorAttributes.g.cs", SourceText.From(markAttributes, Encoding.UTF8)));

            IncrementalValueProvider<ImmutableArray<ConverterTypeInfo>> converterTypes = context.SyntaxProvider.ForAttributeWithMetadataName("ReverseMarkdown.SourceGenerator.ConverterAttribute", (syntaxNode, _) => true, GetConverterTypeName).Where(x => !string.IsNullOrEmpty(x.Name)).Collect();
            IncrementalValuesProvider<ConverterTypesInfoGenerationTarget> generationTargets = context.SyntaxProvider.ForAttributeWithMetadataName("ReverseMarkdown.SourceGenerator.ConverterTypesInfoAttribute", (syntaxNode, _) => true, GetConverterTypesInfoGenerationTargetInfo).Combine(converterTypes).Select(GetConverterTypesInfoGenerationTarget).Where(x => !string.IsNullOrEmpty(x.ClassName));
            context.RegisterSourceOutput(generationTargets, AddSourceForTarget);
        }

        private static ConverterTypeInfo GetConverterTypeName(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            if (!(context.TargetSymbol is INamedTypeSymbol typeSymbol))
            {
                return default;
            }
            if (typeSymbol.IsAbstract || typeSymbol.IsStatic)
            {
                return default;
            }
            bool requiredInterfaceFound = false;
            foreach (INamedTypeSymbol implementedInterfaces in typeSymbol.AllInterfaces)
            {
                // TODO: Improve hacky name extraction.
                if (string.Equals(SymbolDisplay.ToDisplayString(implementedInterfaces), "ReverseMarkdown.Converters.IConverter", StringComparison.Ordinal))
                {
                    requiredInterfaceFound = true;
                    break;
                }
            }
            if (!requiredInterfaceFound)
            {
                return default;
            }
            bool requiredConstructorFound = false;
            foreach (IMethodSymbol constructor in typeSymbol.InstanceConstructors)
            {
                if (constructor.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }
                if (constructor.Parameters.Length != 1)
                {
                    continue;
                }
                if (string.Equals(SymbolDisplay.ToDisplayString(constructor.Parameters[0].Type), "ReverseMarkdown.Converter", StringComparison.Ordinal))
                {
                    requiredConstructorFound = true;
                    break;
                }
            }
            if (!requiredConstructorFound)
            {
                return default;
            }
            return new ConverterTypeInfo(SymbolDisplay.ToDisplayString(typeSymbol), typeSymbol.ContainingAssembly.Name);
        }

        private static ConverterTypesInfoGenerationTargetInfo GetConverterTypesInfoGenerationTargetInfo(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            if (!(context.TargetSymbol is INamedTypeSymbol typeSymbol))
            {
                return default;
            }
            AttributeData attribute = context.Attributes.First();
            string assemblyName = null;
            if (attribute.NamedArguments.Length == 1)
            {
                TypedConstant attributeValue = attribute.NamedArguments[0].Value;
                if (attributeValue.Kind == TypedConstantKind.Primitive && attributeValue.Value is string value)
                {
                    assemblyName = value;
                }
            }
            string namespaceName = GetSymbolNamespaceName(typeSymbol);
            string className = typeSymbol.Name;
            return new ConverterTypesInfoGenerationTargetInfo(className, namespaceName, assemblyName);
        }

        private static ConverterTypesInfoGenerationTarget GetConverterTypesInfoGenerationTarget((ConverterTypesInfoGenerationTargetInfo, ImmutableArray<ConverterTypeInfo>) item, CancellationToken cancellationToken)
        {
            (ConverterTypesInfoGenerationTargetInfo targetInfo, ImmutableArray<ConverterTypeInfo> converterTypes) = item;
            if (string.IsNullOrEmpty(targetInfo.AssemblyName))
            {
                return new ConverterTypesInfoGenerationTarget(targetInfo.ClassName, targetInfo.NamespaceName, converterTypes);
            }
            List<ConverterTypeInfo> filteredConverterTypes = new List<ConverterTypeInfo>();
            foreach (ConverterTypeInfo typeInfo in converterTypes)
            {
                if (string.Equals(targetInfo.AssemblyName, typeInfo.AssemblyName, StringComparison.Ordinal))
                {
                    filteredConverterTypes.Add(typeInfo);
                }
            }
            if (filteredConverterTypes.Count == 0)
            {
                return default;
            }
            return new ConverterTypesInfoGenerationTarget(targetInfo.ClassName, targetInfo.NamespaceName, filteredConverterTypes.ToImmutableArray());
        }

        private static void AddSourceForTarget(SourceProductionContext sourceProductionContext, ConverterTypesInfoGenerationTarget generationTarget)
        {
            string filename = string.IsNullOrEmpty(generationTarget.NamespaceName) ? $"ReverseMarkdownConverterTypesInfo.{generationTarget.ClassName}.g.cs" : $"ReverseMarkdownConverterTypesInfo.{generationTarget.NamespaceName}.{generationTarget.ClassName}.g.cs";
            sourceProductionContext.AddSource(filename, SourceText.From(GetSource(generationTarget), Encoding.UTF8));
        }

        private static string GetSource(ConverterTypesInfoGenerationTarget generationTarget)
        {
            StringBuilder sb = new StringBuilder();
            bool useNamespace = !string.IsNullOrEmpty(generationTarget.NamespaceName);
            if (useNamespace)
            {
                sb.Append($@"
namespace {generationTarget.NamespaceName}
{{");
            }
            string namespaceIndentation = useNamespace ? "    " : string.Empty;
            sb.Append($@"
{namespaceIndentation}partial class {generationTarget.ClassName}
{namespaceIndentation}{{
{namespaceIndentation}    public static global::System.Collections.Generic.IEnumerable<global::ReverseMarkdown.ConverterType> ConverterTypes
{namespaceIndentation}    {{
{namespaceIndentation}        get
{namespaceIndentation}        {{");
            foreach (ConverterTypeInfo converterType in generationTarget.ConverterTypes)
            {
                sb.Append($@"
{namespaceIndentation}            yield return new global::ReverseMarkdown.ConverterType(typeof(global::{converterType.Name}));");
            }
            sb.Append($@"
{namespaceIndentation}        }}
{namespaceIndentation}    }}
{namespaceIndentation}}}");
            if (useNamespace)
            {
                sb.Append(@"
}");
            }
            return sb.ToString();
        }

        private static string GetSymbolNamespaceName(ISymbol symbol) => symbol.ContainingNamespace.IsGlobalNamespace ? null : SymbolDisplay.ToDisplayString(symbol.ContainingNamespace);

        private const string markAttributes = @"
namespace ReverseMarkdown.SourceGenerator
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Class)]
    internal class ConverterTypesInfoAttribute : global::System.Attribute
    {
        public string AssemblyName { get; set; }
    }

    [global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct)]
    internal class ConverterAttribute : global::System.Attribute
    {
    }
}
";
    }
}
