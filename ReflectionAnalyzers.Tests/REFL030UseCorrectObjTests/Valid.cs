﻿namespace ReflectionAnalyzers.Tests.REFL030UseCorrectObjTests;

using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

public static partial class Valid
{
    public static class ConstructorInfoInvoke
    {
        private static readonly InvokeAnalyzer Analyzer = new();
        private static readonly DiagnosticDescriptor Descriptor = Descriptors.REFL030UseCorrectObj;

        [TestCase("GetConstructor(Type.EmptyTypes).Invoke(null)")]
        [TestCase("GetConstructor(new[] { typeof(int) }).Invoke(new object[] { 1 })")]
        public static void InvokeWithOneArgument(string call)
        {
            var code = @"
#pragma warning disable CS8602
namespace N
{
    using System;

    public class C
    {
        public C()
        {
        }

        public C(int value)
        {
        }

        public static C Get(Type unused) => (C)typeof(C).GetConstructor(Type.EmptyTypes).Invoke(null);
    }
}".AssertReplace("GetConstructor(Type.EmptyTypes).Invoke(null)", call);

            RoslynAssert.Valid(Analyzer, Descriptor, code);
        }

        [TestCase("type.GetConstructor(Type.EmptyTypes).Invoke(instance, null)")]
        [TestCase("type.GetConstructor(new[] { typeof(int) }).Invoke(instance, new object[] { 1 })")]
        public static void InvokeWithGetUninitializedObjectAndArgument(string call)
        {
            var code = @"
#pragma warning disable CS8602
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

        public static void M(Type unused)
        {
            var type = typeof(C);
#pragma warning disable SYSLIB0050 // Type or member is obsolete
            var instance = FormatterServices.GetUninitializedObject(type);
#pragma warning restore SYSLIB0050 // Type or member is obsolete
            type.GetConstructor(Type.EmptyTypes).Invoke(instance, null);
        }
    }
}".AssertReplace("type.GetConstructor(Type.EmptyTypes).Invoke(instance, null)", call);

            RoslynAssert.Valid(Analyzer, Descriptor, code);
        }
    }
}
