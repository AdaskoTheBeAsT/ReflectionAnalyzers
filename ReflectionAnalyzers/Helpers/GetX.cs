namespace ReflectionAnalyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for Type.GetField, Type.GetEvent, Type.GetMember, Type.GetMethod...
    /// </summary>
    internal static class GetX
    {
        /// <summary>
        /// Returns Foo for the invocation typeof(Foo).GetProperty(Bar).
        /// </summary>
        /// <param name="invocation">The invocation of a GetX method, GetEvent, GetField etc.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <param name="result">The type.</param>
        /// <returns>True if the type could be determined.</returns>
        internal static bool TryGetTargetType(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out ITypeSymbol result)
        {
            result = null;
            return invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                   TryGetTargetType(memberAccess.Expression, semanticModel, null, cancellationToken, out result);
        }

        private static bool TryGetTargetType(ExpressionSyntax expression, SemanticModel semanticModel, PooledSet<ExpressionSyntax> visited, CancellationToken cancellationToken, out ITypeSymbol result)
        {
            result = null;
            switch (expression)
            {
                case IdentifierNameSyntax identifierName when semanticModel.TryGetSymbol(identifierName, cancellationToken, out ILocalSymbol local):
                    using (visited = visited.IncrementUsage())
                    {
                        return AssignedValueWalker.TryGetSingle(local, semanticModel, cancellationToken, out var assignedValue) &&
                               visited.Add(assignedValue) &&
                               TryGetTargetType(assignedValue, semanticModel, visited, cancellationToken, out result);
                    }

                case TypeOfExpressionSyntax typeOf:
                    return semanticModel.TryGetType(typeOf.Type, cancellationToken, out result);
                case InvocationExpressionSyntax getType when getType.TryGetMethodName(out var name) &&
                                                 name == "GetType" &&
                                                 getType.ArgumentList is ArgumentListSyntax args:
                    if (args.Arguments.Count == 0)
                    {
                        switch (getType.Expression)
                        {
                            case MemberAccessExpressionSyntax typeAccess:
                                return semanticModel.TryGetType(typeAccess.Expression, cancellationToken, out result);
                            case IdentifierNameSyntax _ when expression.TryFirstAncestor(out TypeDeclarationSyntax containingType):
                                return semanticModel.TryGetSymbol(containingType, cancellationToken, out result);
                        }
                    }
                    else if (args.Arguments.TrySingle(out var arg) &&
                             arg.TryGetStringValue(semanticModel, cancellationToken, out var typeName) &&
                             getType.TryGetTarget(KnownSymbol.Assembly.GetType, semanticModel, cancellationToken, out _))
                    {
                        switch (getType.Expression)
                        {
                            case MemberAccessExpressionSyntax typeAccess when semanticModel.TryGetType(typeAccess.Expression, cancellationToken, out var typeInAssembly):
                                result = typeInAssembly.ContainingAssembly.GetTypeByMetadataName(typeName);
                                return result != null;
                            case IdentifierNameSyntax _ when expression.TryFirstAncestor(out TypeDeclarationSyntax containingType) &&
                                                            semanticModel.TryGetSymbol(containingType, cancellationToken, out var typeInAssembly):
                                result = typeInAssembly.ContainingAssembly.GetTypeByMetadataName(typeName);
                                return result != null;
                        }
                    }

                    break;
            }

            return false;
        }
    }
}