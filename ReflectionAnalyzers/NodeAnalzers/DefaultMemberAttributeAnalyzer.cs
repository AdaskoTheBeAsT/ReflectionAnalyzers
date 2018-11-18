namespace ReflectionAnalyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Attribute = Gu.Roslyn.AnalyzerExtensions.Attribute;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DefaultMemberAttributeAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            REFL046DefaultMemberMustExist.Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(x => Handle(x), SyntaxKind.Attribute);
        }

        private static void Handle(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            if (!(context.Node is AttributeSyntax attribute))
            {
                return;
            }

            if (!Attribute.IsType(attribute, KnownSymbol.DefaultMemberAttribute, context.SemanticModel, context.CancellationToken))
            {
                return;
            }

            if (!Attribute.TryFindArgument(attribute, 0, "memberName", out var argument))
            {
                context.ReportDiagnostic(Diagnostic.Create(REFL046DefaultMemberMustExist.Descriptor, argument.GetLocation(), "argument not found"));
                return;
            }

            if (!context.SemanticModel.TryGetConstantValue(argument.Expression, context.CancellationToken, out string memberName))
            {
                context.ReportDiagnostic(Diagnostic.Create(REFL046DefaultMemberMustExist.Descriptor, argument.GetLocation(), "unable to resolve argument value as constant"));
                return;
            }

            if (memberName != "Exists")
            {
                context.ReportDiagnostic(Diagnostic.Create(REFL046DefaultMemberMustExist.Descriptor, argument.GetLocation(), "not equal to \"Exists\""));
            }
        }
    }
}
