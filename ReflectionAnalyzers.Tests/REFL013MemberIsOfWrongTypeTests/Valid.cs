namespace ReflectionAnalyzers.Tests.REFL013MemberIsOfWrongTypeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new GetXAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = REFL013MemberIsOfWrongType.Descriptor;

        [Test]
        public static void PassingArrayToMakeGenericType()
        {
            var code = @"
namespace RoslynSandbox
{
    using System;

    class C<T1, T2>
    {
        void M(Type[] types)
        {
            typeof(C<,>).MakeGenericType(types);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptor, code);
        }
    }
}