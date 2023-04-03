﻿namespace ReflectionAnalyzers.Tests.REFL041CreateDelegateTypeTests;

using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

public static class Valid
{
    private static readonly CreateDelegateAnalyzer Analyzer = new();
    private static readonly DiagnosticDescriptor Descriptor = Descriptors.REFL041CreateDelegateType;

    [Test]
    public static void CreateDelegateParameterExpressionMake()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    class C
    {

        public static object Get => (Func<Type, string, bool, ParameterExpression>)Delegate.CreateDelegate(
            typeof(Func<Type, string, bool, ParameterExpression>),
            typeof(ParameterExpression).GetMethod(""Make"", BindingFlags.Static | BindingFlags.NonPublic));
    }
}";
        RoslynAssert.Valid(Analyzer, Descriptor, code);
    }

    [Test]
    public static void StaticStringInt()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public static int M(string arg) => arg.Length;

        public static object Get => (Func<string, int>)Delegate.CreateDelegate(
            typeof(Func<string, int>),
            typeof(C).GetMethod(nameof(M)));
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void StaticStringIntWithFirstArg()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public static int M(string arg) => arg.Length;

        public static object Get => (Func<int>)Delegate.CreateDelegate(
            typeof(Func<int>),
            string.Empty,
            typeof(C).GetMethod(nameof(M)));
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void StaticVoid()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public static void M() { }

        public static object Get => (Action)Delegate.CreateDelegate(
            typeof(Action),
            typeof(C).GetMethod(nameof(M)));
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void StaticStringVoid()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public static void M(string arg) { }

        public static object Get => (Action<string>)Delegate.CreateDelegate(
            typeof(Action<string>),
            typeof(C).GetMethod(nameof(M)));
    }
}";

        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void StaticStringVoidFirstArg()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public static void M(string arg) { }

        public static object Get => (Action)Delegate.CreateDelegate(
            typeof(Action),
            string.Empty,
            typeof(C).GetMethod(nameof(M)));
    }
}";

        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void StaticStringStringVoidFirstArg()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public static void M(string arg1, string arg2) { }

        public static object Get => (Action<string>)Delegate.CreateDelegate(
            typeof(Action<string>),
            string.Empty,
            typeof(C).GetMethod(nameof(M)));
    }
}";

        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void InstanceStringInt()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public int M(string arg) => arg.Length;

        public static object Get => (Func<C, string, int>)Delegate.CreateDelegate(
            typeof(Func<C, string, int>),
            typeof(C).GetMethod(nameof(M)));
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void InstanceStringIntWithTarget()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        public int M(string arg) => arg.Length;

        public static object Get => (Func<string, int>)Delegate.CreateDelegate(
            typeof(Func<string, int>),
            new C(),
            typeof(C).GetMethod(nameof(M)));
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void StaticStringIntCustomDelegate()
    {
        var code = @"
#pragma warning disable CS8604
namespace N
{
    using System;

    class C
    {
        delegate int StringInt(string text);

        public static int M(string arg) => arg.Length;

        public static object Get => (StringInt)Delegate.CreateDelegate(
            typeof(StringInt),
            typeof(C).GetMethod(nameof(M)));
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void GetGetMethodReturnType()
    {
        var code = @"
#pragma warning disable CS8602, CS8604
namespace N
{
    using System;
    using System.Reflection;

    class C
    {
        // incorrect return type.
        public static Func<C, int> Getter { get; } =
            (Func<C, int>)Delegate.CreateDelegate(
                typeof(Func<C, int>),
                typeof(C).GetProperty(
                    nameof(C.Value),
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).GetGetMethod());

        public int Value { get; set; }
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }

    [Test]
    public static void GetMethodReturnType()
    {
        var code = @"
#pragma warning disable CS8602, CS8604
namespace N
{
    using System;
    using System.Reflection;

    class C
    {
        // incorrect return type.
        public static Func<C, int> Getter { get; } =
            (Func<C, int>)Delegate.CreateDelegate(
                typeof(Func<C, int>),
                typeof(C).GetProperty(
                    nameof(C.Value),
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).GetMethod);

        public int Value { get; set; }
    }
}";
        RoslynAssert.Valid(Analyzer, code);
    }
}
