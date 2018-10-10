namespace ReflectionAnalyzers.Tests.REFL028CastReturnValueToCorrectTypeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;
    using ReflectionAnalyzers.Codefixes;

    public partial class CodeFix
    {
        public class ConstructorInfoInvoke
        {
            private static readonly DiagnosticAnalyzer Analyzer = new InvokeAnalyzer();
            private static readonly CodeFixProvider Fix = new CastReturnValueFix();
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(REFL028CastReturnValueToCorrectType.Descriptor);

            [Test]
            public void WhenCastingToWrongType()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public Foo(int i)
        {
            var value = (↓string)typeof(Foo).GetConstructor(new[] { typeof(int) }).Invoke(new object[] { 1 });
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public Foo(int i)
        {
            var value = (Foo)typeof(Foo).GetConstructor(new[] { typeof(int) }).Invoke(new object[] { 1 });
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, code, fixedCode);
            }
        }
    }
}
