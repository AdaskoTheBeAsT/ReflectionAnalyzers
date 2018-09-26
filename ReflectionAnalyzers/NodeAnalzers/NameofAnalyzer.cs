namespace ReflectionAnalyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NameofAnalyzer : DiagnosticAnalyzer
    {
        private const string Key = nameof(NameSyntax);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            REFL016UseNameof.Descriptor,
            REFL017DontUseNameof.Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => HandleLiteral(c), SyntaxKind.StringLiteralExpression);
            context.RegisterSyntaxNodeAction(c => HandleNameof(c), SyntaxKind.InvocationExpression);
        }

        private static void HandleLiteral(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is LiteralExpressionSyntax literal &&
                literal.Parent is ArgumentSyntax argument &&
                literal.Token.ValueText is string text &&
                SyntaxFacts.IsValidIdentifier(text) &&
                argument.Parent is ArgumentListSyntax argumentList &&
                argumentList.Parent is InvocationExpressionSyntax invocation &&
                TryGetX(invocation, context, out var getX) &&
                TryGetTarget(invocation, getX, text, context, out var target, out var instance) &&
                target.HasValue &&
                TryGetTargetName(target.Value, instance, context, out var targetName))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        REFL016UseNameof.Descriptor,
                        literal.GetLocation(),
                        ImmutableDictionary<string, string>.Empty.Add(Key, $"nameof({targetName})")));
            }
        }

        private static void HandleNameof(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is InvocationExpressionSyntax candidate &&
                IsNameOf(out var argument) &&
                candidate.Parent is ArgumentSyntax containingArgument &&
                containingArgument.TryGetStringValue(context.SemanticModel, context.CancellationToken, out var name) &&
                containingArgument.Parent is ArgumentListSyntax containingArgumentList &&
                containingArgumentList.Parent is InvocationExpressionSyntax invocation &&
                TryGetX(invocation, context, out var getX) &&
                TryGetTarget(invocation, getX, name, context, out var target, out var instance))
            {
                if (!target.HasValue &&
                    !instance.HasValue)
                {
                    if (containingArgument.TryGetStringValue(context.SemanticModel, context.CancellationToken, out name))
                    {
                        context.ReportDiagnostic(
                        Diagnostic.Create(
                            REFL017DontUseNameof.Descriptor,
                            containingArgument.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(nameof(SyntaxKind.StringLiteralExpression), name)));
                    }
                }
                else if (TryGetSymbol(out var symbol) &&
                         !symbol.ContainingType.IsAssignableTo(target.Value.ContainingType, context.Compilation) &&
                         TryGetTargetName(target.Value, default(Optional<IdentifierNameSyntax>), context, out var targetName))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            REFL016UseNameof.Descriptor,
                            argument.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(Key, $"{targetName}")));
                }
            }

            bool TryGetSymbol(out ISymbol symbol)
            {
                return context.SemanticModel.TryGetSymbol(argument.Expression, context.CancellationToken, out symbol) ||
                       context.SemanticModel.GetSymbolInfo(argument.Expression, context.CancellationToken)
                              .CandidateSymbols.TryFirst(out symbol);
            }

            bool IsNameOf(out ArgumentSyntax result)
            {
                result = null;
                return candidate.ArgumentList is ArgumentListSyntax argumentList &&
                       argumentList.Arguments.TrySingle(out result) &&
                       candidate.Expression is IdentifierNameSyntax identifierName &&
                       identifierName.Identifier.ValueText == "nameof";
            }
        }

        private static bool TryGetX(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context, out IMethodSymbol getX)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                !(memberAccess.Expression is InstanceExpressionSyntax))
            {
                return invocation.TryGetTarget(KnownSymbol.Type.GetEvent, context.SemanticModel, context.CancellationToken, out getX) ||
                       invocation.TryGetTarget(KnownSymbol.Type.GetField, context.SemanticModel, context.CancellationToken, out getX) ||
                       invocation.TryGetTarget(KnownSymbol.Type.GetMember, context.SemanticModel, context.CancellationToken, out getX) ||
                       invocation.TryGetTarget(KnownSymbol.Type.GetMethod, context.SemanticModel, context.CancellationToken, out getX) ||
                       invocation.TryGetTarget(KnownSymbol.Type.GetNestedType, context.SemanticModel, context.CancellationToken, out getX) ||
                       invocation.TryGetTarget(KnownSymbol.Type.GetProperty, context.SemanticModel, context.CancellationToken, out getX);
            }

            getX = null;
            return false;
        }

        private static bool TryGetTarget(InvocationExpressionSyntax invocation, IMethodSymbol getX, string name, SyntaxNodeAnalysisContext context, out Optional<ISymbol> target, out Optional<IdentifierNameSyntax> instance)
        {
            instance = default(Optional<IdentifierNameSyntax>);
            target = default(Optional<ISymbol>);
            if (GetX.TryGetType(invocation, context, out var targetType, out var typeSource))
            {
                if (typeSource.HasValue &&
                    typeSource.Value is InvocationExpressionSyntax getType &&
                    getType.TryGetTarget(KnownSymbol.Object.GetType, context.SemanticModel, context.CancellationToken, out _) &&
                    getType.Expression is MemberAccessExpressionSyntax memberAccess &&
                    memberAccess.Expression is IdentifierNameSyntax identifierName)
                {
                    instance = new Optional<IdentifierNameSyntax>(identifierName);
                }

                _ = GetX.TryGetMember(getX, targetType, name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy, GetX.AnyTypes, context, out var targetSymbol);

                if (targetSymbol is IMethodSymbol method)
                {
                    target = method.AssociatedSymbol == null
                        ? new Optional<ISymbol>(targetSymbol)
                        : default(Optional<ISymbol>);
                }
                else
                {
                    target = targetSymbol != null
                        ? new Optional<ISymbol>(targetSymbol)
                        : default(Optional<ISymbol>);
                }

                return true;
            }

            return false;
        }

        private static bool TryGetTargetName(ISymbol symbol, Optional<IdentifierNameSyntax> instance, SyntaxNodeAnalysisContext context, out string name)
        {
            name = null;
            if (instance.HasValue)
            {
                name = $"{instance.Value}.{symbol.Name}";
                return true;
            }

            if (!context.SemanticModel.IsAccessible(context.Node.SpanStart, symbol) ||
                symbol.ContainingType.IsAnonymousType)
            {
                return false;
            }

            if (symbol.ContainingType.IsAssignableTo(context.ContainingSymbol.ContainingType, context.Compilation))
            {
                name = symbol.IsStatic ||
                       symbol is ITypeSymbol ||
                       IsStaticContext(context)
                    ? symbol.Name
                    : context.SemanticModel.UnderscoreFields() ? symbol.Name : $"this.{symbol.Name}";
                return true;
            }

            name = context.SemanticModel.IsAccessible(context.Node.SpanStart, symbol)
                ? $"{symbol.ContainingType.ToMinimalDisplayString(context.SemanticModel, context.Node.SpanStart)}.{symbol.Name}"
                : $"\"{symbol.Name}\"";
            return true;
        }

        private static bool IsStaticContext(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.TryFirstAncestor(out AccessorDeclarationSyntax accessor))
            {
                return context.SemanticModel.GetDeclaredSymbolSafe(accessor.FirstAncestor<PropertyDeclarationSyntax>(), context.CancellationToken)?.IsStatic != false;
            }

            if (context.Node.TryFirstAncestor(out BaseMethodDeclarationSyntax methodDeclaration))
            {
                return context.SemanticModel.GetDeclaredSymbolSafe(methodDeclaration, context.CancellationToken)?.IsStatic != false;
            }

            return !context.Node.TryFirstAncestor<AttributeArgumentListSyntax>(out _);
        }
    }
}
