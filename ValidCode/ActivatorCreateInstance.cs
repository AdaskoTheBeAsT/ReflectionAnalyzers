// ReSharper disable All
namespace ValidCode;

using System;
using NUnit.Framework;

public class ActivatorCreateInstance
{
    [Test]
    public void Valid()
    {
        Assert.That(Activator.CreateInstance<ImplicitDefaultConstructor>(), Is.Not.Null);
        Assert.That((ImplicitDefaultConstructor?)Activator.CreateInstance(typeof(ImplicitDefaultConstructor)), Is.Not.Null);
        Assert.That((ImplicitDefaultConstructor?)Activator.CreateInstance(typeof(ImplicitDefaultConstructor), true), Is.Not.Null);
        Assert.That((ImplicitDefaultConstructor?)Activator.CreateInstance(typeof(ImplicitDefaultConstructor), false), Is.Not.Null);

        Assert.That(Activator.CreateInstance<ExplicitDefaultConstructor>(), Is.Not.Null);
        Assert.That((ExplicitDefaultConstructor?)Activator.CreateInstance(typeof(ExplicitDefaultConstructor)), Is.Not.Null);
        Assert.That((ExplicitDefaultConstructor?)Activator.CreateInstance(typeof(ExplicitDefaultConstructor), true), Is.Not.Null);
        Assert.That((ExplicitDefaultConstructor?)Activator.CreateInstance(typeof(ExplicitDefaultConstructor), false), Is.Not.Null);

        Assert.That((PrivateDefaultConstructor?)Activator.CreateInstance(typeof(PrivateDefaultConstructor), true), Is.Not.Null);

        Assert.That((SingleDoubleParameter?)Activator.CreateInstance(typeof(SingleDoubleParameter), 1), Is.Not.Null);
        Assert.That((SingleDoubleParameter?)Activator.CreateInstance(typeof(SingleDoubleParameter), 1.2), Is.Not.Null);
        Assert.That((SingleDoubleParameter?)Activator.CreateInstance(typeof(SingleDoubleParameter), new object[] { 1 }), Is.Not.Null);
        Assert.That((SingleDoubleParameter?)Activator.CreateInstance(typeof(SingleDoubleParameter), new object[] { 1.2 }), Is.Not.Null);
    }

    public T Create<T>() => Activator.CreateInstance<T>();

    public static object? Foo<T>(object _) => (T)Activator.CreateInstance(typeof(T), "foo")!;

    public static object? Foo<T>() => (T?)Activator.CreateInstance(typeof(T));

    public class ImplicitDefaultConstructor
    {
    }

    public class ExplicitDefaultConstructor
    {
        public ExplicitDefaultConstructor()
        {
        }
    }

    public class PrivateDefaultConstructor
    {
        private PrivateDefaultConstructor()
        {
        }
    }

    public class SingleDoubleParameter
    {
        public SingleDoubleParameter(double value)
        {
        }
    }
}
