﻿namespace ReflectionAnalyzers.Tests.REFL002DiscardReturnValueTests;

using Gu.Roslyn.Asserts;
using NUnit.Framework;

public static class Diagnostics
{
    private static readonly InvokeAnalyzer Analyzer = new();
    private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.REFL002DiscardReturnValue);

    [Test]
    public static void AssigningLocal()
    {
        var code = @"
#pragma warning disable CS8602
namespace N
{
    public class C
    {
        public C()
        {
            var value = ↓typeof(C).GetMethod(nameof(M)).Invoke(null, null);
        }

        public static void M()
        {
        }
    }
}";

        RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, code);
    }

    [Test]
    public static void AssigningField()
    {
        var code = @"
#pragma warning disable CS8602, CS8605
namespace N
{
    public class C
    {
        private readonly int value;

        public C()
        {
            this.value = (int)↓typeof(C).GetMethod(nameof(M)).Invoke(null, null);
        }

        public static void M()
        {
        }
    }
}";

        RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, code);
    }

    [Test]
    public static void UsingInExpression()
    {
        var code = @"
#pragma warning disable CS8602, CS8605
namespace N
{
    public class C
    {
        public C()
        {
            var text = ((int)↓typeof(C).GetMethod(nameof(M)).Invoke(null, null)).ToString();
        }

        public static void M()
        {
        }
    }
}";

        RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, code);
    }

    [TestCase("type.GetConstructor(Type.EmptyTypes).Invoke(instance, null)")]
    [TestCase("type.GetConstructor(new[] { typeof(int) }).Invoke(instance, new object[] { 1 })")]
    public static void InvokeWithGetUninitializedObjectAndArgument(string call)
    {
        var code = @"
#pragma warning disable CS8602, CS8605, SYSLIB0050
namespace N
{
    using System;
    using System.Runtime.Serialization;

    public class C
    {
        public C()
        {
        }

        public C(int value)
        {
        }

        public C(Type type)
        {
        }

        public static void M()
        {
            var type = typeof(C);
            var instance = FormatterServices.GetUninitializedObject(type);
            var c = type.GetConstructor(Type.EmptyTypes).Invoke(instance, null);
        }
    }
}".AssertReplace("type.GetConstructor(Type.EmptyTypes).Invoke(instance, null)", call);

        RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, code);
    }
}
