// ReSharper disable All
namespace ValidCode;

using System;
using System.Reflection;
using NUnit.Framework;
using static System.Reflection.BindingFlags;

class UsingStatic
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(UsingStatic).GetMethod(nameof(this.Bar), Public | Instance | DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(typeof(UsingStatic).GetMethod(nameof(this.Bar), Public | BindingFlags.Instance | DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(typeof(UsingStatic).GetMethod(nameof(this.Bar), Public | System.Reflection.BindingFlags.Instance | DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
    }

    public int Bar() => 0;
}
