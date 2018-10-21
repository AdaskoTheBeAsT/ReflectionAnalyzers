namespace ReflectionAnalyzers
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal static class AssemblyExt
    {
        internal static INamedTypeSymbol GetTypeByMetadataName(this IAssemblySymbol assembly, TypeNameArgument typeName, bool ignoreCase)
        {
            if (typeName.TryGetGeneric(out var generic))
            {
                return GetTypeByMetadataName(assembly, generic, ignoreCase);
            }

            return GetTypeByMetadataName(assembly, typeName.Value, ignoreCase);
        }

        internal static INamedTypeSymbol GetTypeByMetadataName(this IAssemblySymbol assembly, string fullyQualifiedMetadataName, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return assembly.GetTypeByMetadataName(fullyQualifiedMetadataName);
            }

            return assembly.GetTypeByMetadataName(fullyQualifiedMetadataName) ??
                   GetTypeByMetadataNameIgnoreCase(assembly.GlobalNamespace);

            INamedTypeSymbol GetTypeByMetadataNameIgnoreCase(INamespaceSymbol ns)
            {
                foreach (var member in ns.GetTypeMembers())
                {
                    if (fullyQualifiedMetadataName.EndsWith(member.MetadataName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(member.QualifiedMetadataName(), fullyQualifiedMetadataName, StringComparison.OrdinalIgnoreCase))
                    {
                        return member;
                    }
                }

                foreach (var nested in ns.GetNamespaceMembers())
                {
                    if (GetTypeByMetadataNameIgnoreCase(nested) is INamedTypeSymbol match)
                    {
                        return match;
                    }
                }

                return null;
            }
        }

        private static INamedTypeSymbol GetTypeByMetadataName(this IAssemblySymbol assembly, GenericTypeName genericTypeName, bool ignoreCase)
        {
            if (TryGetArgsTypes(out var args))
            {
                return assembly.GetTypeByMetadataName(genericTypeName.MetadataName, ignoreCase).Construct(args);
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
