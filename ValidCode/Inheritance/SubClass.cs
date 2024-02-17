// ReSharper disable All
namespace ValidCode.Inheritance;

using System;
using System.Reflection;
using NUnit.Framework;

public class SubClass : BaseClass
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(SubClass).GetField(nameof(BaseClass.PublicStaticField), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), Is.Not.Null);
        Assert.That(typeof(SubClass).GetEvent(nameof(BaseClass.PublicStaticEvent), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), Is.Not.Null);
        Assert.That(typeof(SubClass).GetProperty(nameof(BaseClass.PublicStaticProperty), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), Is.Not.Null);
        Assert.That(typeof(SubClass).GetMethod(nameof(BaseClass.PublicStaticMethod), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, Type.EmptyTypes, null), Is.Not.Null);

        Assert.That(typeof(BaseClass).GetField(nameof(BaseClass.PublicStaticField), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(BaseClass).GetField("PrivateStaticField", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(BaseClass).GetEvent(nameof(BaseClass.PublicStaticEvent), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(BaseClass).GetEvent("PrivateStaticEvent", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(BaseClass).GetProperty(nameof(BaseClass.PublicStaticProperty), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(BaseClass).GetProperty("PrivateStaticProperty", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(BaseClass).GetMethod(nameof(BaseClass.PublicStaticMethod), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(typeof(BaseClass).GetMethod("PrivateStaticMethod", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
    }
}
