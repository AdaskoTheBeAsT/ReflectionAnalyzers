// ReSharper disable All
namespace ValidCode;

using System;
using System.Reflection;
using NUnit.Framework;

public class MakeGenericType
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(Foo<>).MakeGenericType(typeof(int)), Is.Not.Null);
        Assert.That(typeof(Foo<>).MakeGenericType(typeof(string)), Is.Not.Null);
        Assert.That(typeof(Foo<>.Bar).MakeGenericType(typeof(int)), Is.Not.Null);
        Assert.That(typeof(ConstrainedToIComparableOfT<>).MakeGenericType(typeof(int)), Is.Not.Null);
        Assert.That(typeof(ConstrainedToClass<>).MakeGenericType(typeof(string)), Is.Not.Null);
        Assert.That(typeof(ConstrainedToStruct<>).MakeGenericType(typeof(int)), Is.Not.Null);
    }

    public Type GetTernary<T>()
    {
        return typeof(T).IsValueType
            ? typeof(MakeGenericType).GetNestedType("ConstrainedToStruct`1", BindingFlags.Public)!.MakeGenericType(typeof(T))
            : typeof(MakeGenericType).GetNestedType("ConstrainedToClass`1", BindingFlags.Public)!.MakeGenericType(typeof(T));
    }

    public Type GetIfReturn<T>()
    {
        if (typeof(T).IsValueType)
        {
            return typeof(MakeGenericType).GetNestedType("ConstrainedToStruct`1", BindingFlags.Public)!.MakeGenericType(typeof(T));
        }

        return typeof(MakeGenericType).GetNestedType("ConstrainedToClass`1", BindingFlags.Public)!.MakeGenericType(typeof(T));
    }

    public Type GetIfElse<T>()
    {
        if (typeof(T).IsValueType)
        {
            return typeof(MakeGenericType).GetNestedType("ConstrainedToStruct`1", BindingFlags.Public)!.MakeGenericType(typeof(T));
        }
        else
        {
            return typeof(MakeGenericType).GetNestedType("ConstrainedToClass`1", BindingFlags.Public)!.MakeGenericType(typeof(T));
        }
    }

    public class Foo<T>
    {
        public class Bar
        {
        }
    }

    public class ConstrainedToIComparableOfT<T>
        where T : IComparable<T>
    {
    }

    public class ConstrainedToClass<T>
        where T : class
    {
    }

    public class ConstrainedToStruct<T>
        where T : struct
    {
    }
}
