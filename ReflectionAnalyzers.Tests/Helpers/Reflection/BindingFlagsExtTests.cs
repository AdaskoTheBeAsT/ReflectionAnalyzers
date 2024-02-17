namespace ReflectionAnalyzers.Tests.Helpers.Reflection;

using System;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class BindingFlagsExtTests
{
    private static readonly BindingFlags[] Flags = Enum.GetValues<BindingFlags>();

    [TestCaseSource(nameof(Flags))]
    public static void ToDisplayString(object flags)
    {
        var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    class C
    {
    }
}");
        Assert.That(((BindingFlags)flags).ToDisplayString(tree.FindClassDeclaration("C")), Is.EqualTo("BindingFlags." + flags));
    }

    [TestCaseSource(nameof(Flags))]
    public static void ToDisplayStringUsingStaticInside(object flags)
    {
        var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using static System.Reflection.BindingFlags;

    class C
    {
    }
}");
        Assert.That(((BindingFlags)flags).ToDisplayString(tree.FindClassDeclaration("C")), Is.EqualTo(flags.ToString()));
    }

    [TestCaseSource(nameof(Flags))]
    public static void ToDisplayStringUsingStaticOutside(object flags)
    {
        var tree = CSharpSyntaxTree.ParseText(@"
using static System.Reflection.BindingFlags;

namespace N
{
    class C
    {
    }
}");
        Assert.That(((BindingFlags)flags).ToDisplayString(tree.FindClassDeclaration("C")), Is.EqualTo(flags.ToString()));
    }
}
