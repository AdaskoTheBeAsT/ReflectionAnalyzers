﻿namespace ReflectionAnalyzers.Tests.REFL028CastReturnValueToCorrectTypeTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class MethodInfoInvoke
        {
            private static readonly InvokeAnalyzer Analyzer = new();
            private static readonly CastReturnValueFix Fix = new();
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.REFL028CastReturnValueToCorrectType);

            [Test]
            public static void WhenCastingToWrongType()
            {
                var before = @"
namespace N
{
    using System;

    public class C
    {
        public static object Get => (↓string)typeof(C).GetMethod(nameof(M)).Invoke(null, new object[] { 1 });

        public static int M(int i) => i;
    }
}";

                var after = @"
namespace N
{
    using System;

    public class C
    {
        public static object Get => (int)typeof(C).GetMethod(nameof(M)).Invoke(null, new object[] { 1 });

        public static int M(int i) => i;
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
            }
        }
    }
}
