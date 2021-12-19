﻿namespace ReflectionAnalyzers;

using System.Threading;

using Gu.Roslyn.AnalyzerExtensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal readonly struct GetMethod
{
    internal readonly InvocationExpressionSyntax Invocation;
    internal readonly IMethodSymbol Target;
    internal readonly ReflectedMember Member;
    internal readonly Name Name;
    internal readonly Flags Flags;
    internal readonly Types Types;

    private GetMethod(InvocationExpressionSyntax invocation, IMethodSymbol target, ReflectedMember member, Name name, Flags flags, Types types)
    {
        this.Invocation = invocation;
        this.Target = target;
        this.Member = member;
        this.Name = name;
        this.Flags = flags;
        this.Types = types;
    }

    internal IMethodSymbol? Single => this.Member.Match == FilterMatch.Single ? (IMethodSymbol)this.Member.Symbol! : null;

    /// <summary>
    /// Check if <paramref name="candidate"/> is a call to Type.GetMethod.
    /// </summary>
    internal static GetMethod? Match(ExpressionSyntax candidate, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (GetX.FindInvocation(candidate, semanticModel, cancellationToken) is { } invocation)
        {
            return Match(invocation, semanticModel, cancellationToken);
        }

        return null;
    }

    /// <summary>
    /// Check if <paramref name="candidate"/> is a call to Type.GetMethod.
    /// </summary>
    internal static GetMethod? Match(InvocationExpressionSyntax candidate, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (candidate.TryGetTarget(KnownSymbol.Type.GetMethod, semanticModel, cancellationToken, out var target))
        {
            if (ReflectedMember.TryGetType(candidate, semanticModel, cancellationToken, out var type, out var typeSource) &&
                Name.TryCreate(candidate, target, semanticModel, cancellationToken, out var name) &&
                Flags.TryCreate(candidate, target, semanticModel, cancellationToken, out var flags) &&
                Types.TryCreate(candidate, target, semanticModel, cancellationToken, out var types) &&
                ReflectedMember.TryCreate(target, candidate, type, typeSource, name, flags.Effective, types, semanticModel.Compilation, out var member))
            {
                return new GetMethod(candidate, target, member, name, flags, types);
            }

            if (Flags.TryCreate(candidate, target, semanticModel, cancellationToken, out flags) &&
                flags.AreInSufficient)
            {
                _ = Name.TryCreate(candidate, target, semanticModel, cancellationToken, out name);
                _ = Types.TryCreate(candidate, target, semanticModel, cancellationToken, out types);
                member = new ReflectedMember(type, typeSource, null, target, candidate, FilterMatch.InSufficientFlags);
                return new GetMethod(candidate, target, member, name, flags, types);
            }
        }

        return null;
    }
}
