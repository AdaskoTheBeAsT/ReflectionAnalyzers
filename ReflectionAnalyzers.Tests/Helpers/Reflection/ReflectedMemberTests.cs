namespace ReflectionAnalyzers.Tests.Helpers.Reflection;

using System;
using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class ReflectedMemberTests
{
    [TestCase("typeof(C).GetMethod(nameof(this.ToString))",                                                                               "C",           "typeof(C)")]
    [TestCase("new C().GetType().GetMethod(nameof(this.ToString))",                                                                       "C",           "new C().GetType()")]
    [TestCase("foo.GetType().GetMethod(nameof(this.ToString))",                                                                           "C",           "foo.GetType()")]
    [TestCase("this.GetType().GetMethod(nameof(this.ToString))",                                                                          "C",           "this.GetType()")]
    [TestCase("GetType().GetMethod(nameof(this.ToString))",                                                                               "C",           "GetType()")]
    [TestCase("typeof(string).Assembly.GetType(\"System.Int32\").GetMethod(nameof(this.ToString))",                                       "Int32",         "typeof(string).Assembly.GetType(\"System.Int32\")")]
    [TestCase("typeof(IEnumerable<int>).Assembly.GetType(\"System.Collections.Generic.IEnumerable`1\").GetMethod(nameof(this.ToString))", "IEnumerable`1", "typeof(IEnumerable<int>).Assembly.GetType(\"System.Collections.Generic.IEnumerable`1\")")]
    public static void TryGetTypeFromExpression(string call, string expected, string expectedSource)
    {
        var code = @"
namespace N
{
    using System.Collections.Generic;
    using System.Reflection;

    class C
    {
        public C(C foo)
        {
            var methodInfo = typeof(C).GetMethod(nameof(this.ToString));
        }
    }
}".AssertReplace("typeof(C).GetMethod(nameof(this.ToString))", call);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindInvocation(call);
        Assert.That(ReflectedMember.TryGetType(node, semanticModel, CancellationToken.None, out var type, out var source), Is.True);
        Assert.That(type.MetadataName, Is.EqualTo(expected));
        Assert.That(source.ToString(), Is.EqualTo(expectedSource));
    }

    [TestCase("typeof(C)", "C")]
    [TestCase("new C().GetType()", "C")]
    [TestCase("foo.GetType()", "C")]
    [TestCase("this.GetType()", "C")]
    [TestCase("GetType()", "C")]
    [TestCase("typeof(string).Assembly.GetType(\"System.Int32\")", "Int32")]
    [TestCase("typeof(IEnumerable<int>).Assembly.GetType(\"System.Collections.Generic.IEnumerable`1\")", "IEnumerable`1")]
    public static void TryGetTypeFromLocal(string typeExpression, string expected)
    {
        var code = @"
namespace N
{
    using System.Collections.Generic;
    using System.Reflection;

    class C
    {
        public C(C foo)
        {
            var type = typeof(C);
            var methodInfo = type.GetMethod(nameof(this.ToString));
        }
    }
}".AssertReplace("typeof(C)", typeExpression);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindInvocation("GetMethod");
        Assert.That(ReflectedMember.TryGetType(node, semanticModel, CancellationToken.None, out var type, out var instance), Is.True);
        Assert.That(type.MetadataName, Is.EqualTo(expected));
        Assert.That(instance.ToString(), Is.EqualTo(typeExpression));
    }

    [Test]
    public static void Recursion()
    {
        var code = @"
namespace N
{
    using System;

    class C
    {
        public C()
        {
            Type type;
            type = type;
            var methodInfo = type.GetMethod(nameof(this.ToString));
        }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var node = syntaxTree.FindInvocation("GetMethod");
        Assert.That(ReflectedMember.TryGetType(node, semanticModel, CancellationToken.None, out _, out _), Is.False);
    }

    [Test]
    public static void Dump()
    {
        Console.WriteLine(typeof(string).Assembly.GetType("System.Int32"));
    }
}
