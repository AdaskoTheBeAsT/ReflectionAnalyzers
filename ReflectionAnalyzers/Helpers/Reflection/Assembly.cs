﻿namespace ReflectionAnalyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class Assembly
    {
        internal static bool HasVisibleTypes(this IAssemblySymbol assembly)
        {
            if (assembly.Locations.Any(x => x.IsInSource))
            {
                return true;
            }

            return false;
        }

        internal static bool TryGet(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken, [NotNullWhen(true)] out IAssemblySymbol? assembly)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax { Name: IdentifierNameSyntax { Identifier: { ValueText: "GetType" } } } candidate:
                    return TryGet(candidate.Expression, semanticModel, cancellationToken, out assembly);
                case MemberAccessExpressionSyntax { Name: IdentifierNameSyntax { Identifier: { ValueText: "Assembly" } } } candidate
                    when Type.TryGet(candidate.Expression, semanticModel, cancellationToken, out var typeInAssembly, out _):
                    assembly = typeInAssembly.ContainingAssembly;
                    return assembly != null;
            }

            assembly = null;
            return false;
        }

        internal static INamedTypeSymbol? GetTypeByMetadataName(this IAssemblySymbol assembly, TypeNameArgument typeName, bool ignoreCase)
        {
            if (typeName.TryGetGeneric(out var generic))
            {
                return GetTypeByMetadataName(assembly, generic, ignoreCase);
            }

            return GetTypeByMetadataName(assembly, typeName.Value, ignoreCase);
        }

        internal static INamedTypeSymbol? GetTypeByMetadataName(this IAssemblySymbol assembly, string fullyQualifiedMetadataName, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return assembly.GetTypeByMetadataName(fullyQualifiedMetadataName);
            }

            return assembly.GetTypeByMetadataName(fullyQualifiedMetadataName) ??
                   GetTypeByMetadataNameIgnoreCase(assembly.GlobalNamespace);

            INamedTypeSymbol? GetTypeByMetadataNameIgnoreCase(INamespaceSymbol ns)
            {
                if (fullyQualifiedMetadataName.StartsWith(ns.MetadataName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var candidate in ns.GetTypeMembers())
                    {
                        if (fullyQualifiedMetadataName.EndsWith(candidate.MetadataName, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(candidate.QualifiedMetadataName(), fullyQualifiedMetadataName, StringComparison.OrdinalIgnoreCase))
                        {
                            return candidate;
                        }
                    }

                    foreach (var nested in ns.GetNamespaceMembers())
                    {
                        if (GetTypeByMetadataNameIgnoreCase(nested) is { } match)
                        {
                            return match;
                        }
                    }
                }

                return null;
            }
        }

        private static INamedTypeSymbol? GetTypeByMetadataName(this IAssemblySymbol assembly, GenericTypeName genericTypeName, bool ignoreCase)
        {
            if (TryGetArgsTypes(out var args))
            {
                return assembly.GetTypeByMetadataName(genericTypeName.MetadataName, ignoreCase)?.Construct(args);
            }

            return null;

            bool TryGetArgsTypes(out ITypeSymbol[] result)
            {
                result = new ITypeSymbol[genericTypeName.TypeArguments.Length];
                for (var i = 0; i < genericTypeName.TypeArguments.Length; i++)
                {
                    var argument = genericTypeName.TypeArguments[i];
                    if (argument.TypeArguments == null)
                    {
                        var type = GetTypeByMetadataName(assembly, argument.MetadataName, ignoreCase);
                        if (type == null)
                        {
                            return false;
                        }

                        result[i] = type;
                    }
                    else
                    {
                        var type = GetTypeByMetadataName(assembly, new GenericTypeName(argument.MetadataName, argument.TypeArguments.ToImmutableArray()), ignoreCase);
                        if (type == null)
                        {
                            return false;
                        }

                        result[i] = type;
                    }
                }

                return true;
            }
        }
    }
}
