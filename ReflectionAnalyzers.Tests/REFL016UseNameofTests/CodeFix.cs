namespace ReflectionAnalyzers.Tests.REFL016UseNameofTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new GetXAnalyzer();
        private static readonly CodeFixProvider Fix = new NameofFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(REFL016UseNameof.DiagnosticId);

        [Test]
        public static void GetPropertyInstance()
        {
            var before = @"
namespace RoslynSandbox
{
    class C
    {
        public C()
        {
            var member = this.GetType().GetProperty(""Bar"");
        }

         public int Bar { get; }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class C
    {
        public C()
        {
            var member = this.GetType().GetProperty(nameof(this.Bar));
        }

         public int Bar { get; }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void GetPropertyInstanceWithTrivia()
        {
            var before = @"
namespace RoslynSandbox
{
    using System.Reflection;

    class C
    {
        public C()
        {
            var member = this.GetType().GetProperty(
  /* trivia1 */ ""Bar""  ,    // trivia2
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

         public int Bar { get; }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System.Reflection;

    class C
    {
        public C()
        {
            var member = this.GetType().GetProperty(
  /* trivia1 */ nameof(this.Bar)  ,    // trivia2
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

         public int Bar { get; }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void GetPropertyStatic()
        {
            var before = @"
namespace RoslynSandbox
{
    class C
    {
        public C()
        {
            var member = this.GetType().GetProperty(""Bar"");
        }

         public static int Bar { get; }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class C
    {
        public C()
        {
            var member = this.GetType().GetProperty(nameof(Bar));
        }

         public static int Bar { get; }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void AnonymousType()
        {
            var before = @"
namespace RoslynSandbox
{
    class C
    {
        public C()
        {
            var anon = new { C = 1 };
            var member = anon.GetType().GetProperty(""C"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class C
    {
        public C()
        {
            var anon = new { C = 1 };
            var member = anon.GetType().GetProperty(nameof(anon.C));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after, fixTitle: "Use nameof(anon.C).");
        }

        [Test]
        public static void TypeofDictionaryGetMethodStringLiteral()
        {
            var before = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;

    public class C
    {
        public C()
        {
            var member = typeof(Dictionary<string, object>).GetMethod(↓""Add"");
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System.Collections.Generic;

    public class C
    {
        public C()
        {
            var member = typeof(Dictionary<string, object>).GetMethod(nameof(Dictionary<string, object>.Add));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after, fixTitle: "Use nameof(Dictionary<string, object>.Add).");
        }

        [TestCase("Class")]
        [TestCase("Enum")]
        [TestCase("Interface")]
        [TestCase("Struct")]
        public static void GetNestedTypePrivateInSameType(string type)
        {
            var before = @"
namespace RoslynSandbox
{
    using System.Reflection;

    public class C
    {
        public C()
        {
            var member = this.GetType().GetNestedType(↓""Class"", BindingFlags.NonPublic);
        }

        private class Class { }

        private enum Enum { }

        private interface Interface { }

        private struct Struct { }
    }
}".AssertReplace("GetNestedType(↓\"Class\", BindingFlags.NonPublic)", $"GetNestedType(↓\"{type}\", BindingFlags.NonPublic)");

            var after = @"
namespace RoslynSandbox
{
    using System.Reflection;

    public class C
    {
        public C()
        {
            var member = this.GetType().GetNestedType(nameof(Class), BindingFlags.NonPublic);
        }

        private class Class { }

        private enum Enum { }

        private interface Interface { }

        private struct Struct { }
    }
}".AssertReplace("nameof(Class)", $"nameof({type})");

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [TestCase("Class")]
        [TestCase("Enum")]
        [TestCase("Interface")]
        [TestCase("Struct")]
        public static void GetNestedTypePublicInOtherType(string type)
        {
            var before = @"
namespace RoslynSandbox
{
    using System.Reflection;

    public class C
    {
        public class Class { }

        public enum Enum { }

        public interface Interface { }

        public struct Struct { }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System.Reflection;

    public class Bar
    {
        public Bar()
        {
            var member = typeof(C).GetNestedType(↓""Class"", BindingFlags.Public);
        }
    }
}".AssertReplace("GetNestedType(↓\"Class\", BindingFlags.Public)", $"GetNestedType(↓\"{type}\", BindingFlags.Public)");

            var after = @"
namespace RoslynSandbox
{
    using System.Reflection;

    public class Bar
    {
        public Bar()
        {
            var member = typeof(C).GetNestedType(nameof(C.Class), BindingFlags.Public);
        }
    }
}".AssertReplace("nameof(C.Class)", $"nameof(C.{type})");

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { before, testCode }, after);
        }

        [Test]
        public static void AggregateExceptionMessage()
        {
            var before = @"
namespace RoslynSandbox
{
    using System;
    using System.Reflection;

    class C
    {
        public C()
        {
            var member = typeof(AggregateException).GetProperty(""Message"", BindingFlags.Public | BindingFlags.Instance);
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reflection;

    class C
    {
        public C()
        {
            var member = typeof(AggregateException).GetProperty(nameof(Exception.Message), BindingFlags.Public | BindingFlags.Instance);
        }
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void SystemWindowsFormsControlCreateControl()
        {
            var before = @"
namespace RoslynSandbox
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    class C
    {
        public object Get => typeof(Control).GetMethod(""CreateControl"", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool) }, null);
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    class C
    {
        public object Get => typeof(Control).GetMethod(nameof(Control.CreateControl), BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool) }, null);
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void NullableGetProperty()
        {
            var before = @"
namespace RoslynSandbox
{
    using System;
    using System.Reflection;

    class C
    {
        public object Get => typeof(int?).GetProperty(""Value"");
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reflection;

    class C
    {
        public object Get => typeof(int?).GetProperty(nameof(Nullable<int>.Value));
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void ValueTupleGetFieldItem1()
        {
            var before = @"
namespace RoslynSandbox
{
    using System;

    class C
    {
        public object Get => typeof((int, double)).GetField(""Item1"");
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System;

    class C
    {
        public object Get => typeof((int, double)).GetField(nameof(ValueTuple<int, double>.Item1));
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void ValueTupleGetFieldRest()
        {
            var before = @"
namespace RoslynSandbox
{
    using System;

    class C
    {
        public object Get => typeof((int, int, int, int, int, int, int, int)).GetField(""Rest"");
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System;

    class C
    {
        public object Get => typeof((int, int, int, int, int, int, int, int)).GetField(nameof(ValueTuple<int, int, int, int, int, int, int, ValueTuple<int>>.Rest));
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, before, after);
        }

        [Test]
        public static void ProtectedMemberInBase()
        {
            var baseCode = @"
namespace RoslynSandbox
{
    class BaseClass
    {
        protected void ProtectedMember() { }
    }
}";

            var before = @"
namespace RoslynSandbox
{
    class C : BaseClass
    {
        public object Get => typeof(BaseClass).GetMethod(↓""ProtectedMember"");
    }
}";

            var after = @"
namespace RoslynSandbox
{
    class C : BaseClass
    {
        public object Get => typeof(BaseClass).GetMethod(nameof(this.ProtectedMember));
    }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { baseCode, before }, after);
        }
    }
}
