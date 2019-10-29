namespace ReflectionAnalyzers.Tests.REFL044ExpectedAttributeTypeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static partial class Valid
    {
        public static class IsDefined
        {
            private static readonly DiagnosticAnalyzer Analyzer = new IsDefinedAnalyzer();
            private static readonly DiagnosticDescriptor Descriptor = REFL044ExpectedAttributeType.Descriptor;

            [TestCase("Attribute")]
            [TestCase("ObsoleteAttribute")]
            public static void AttributeIsDefined(string type)
            {
                var code = @"
namespace N
{
    using System;

    class C
    {
        public static bool M() => Attribute.IsDefined(typeof(C), typeof(Attribute));
    }
}".AssertReplace("Attribute", type);

                RoslynAssert.Valid(Analyzer, Descriptor, code);
            }

            [Test]
            public static void AttributeIsDefinedGeneric()
            {
                var code = @"
namespace N
{
    using System;

    class C
    {
        public static bool M<T>() 
            where T : Attribute
        {
            return Attribute.IsDefined(typeof(C), typeof(T));
        }
    }
}";

                RoslynAssert.Valid(Analyzer, Descriptor, code);
            }
        }
    }
}
