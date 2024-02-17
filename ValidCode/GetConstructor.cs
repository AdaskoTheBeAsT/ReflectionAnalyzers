// ReSharper disable All
namespace ValidCode;

using System;
using System.Reflection;
using NUnit.Framework;

public class GetConstructor
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(Default).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(typeof(Single).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(typeof(Two).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(typeof(Two).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(double) }, null), Is.Not.Null);
    }

    public class Default
    {
    }

    public class Single
    {
        public Single()
        {
        }
    }

    public class Two
    {
        public Two(int value)
        {
        }

        public Two(double value)
        {
        }
    }
}
