// ReSharper disable All
namespace ValidCode;

using System;
using System.Reflection;

using NUnit.Framework;

public class Accessors
{
#pragma warning disable CS0067
    public event EventHandler? E;

    public int P { get; set; }

    [Test]
    public void Valid()
    {
        var instance = new Accessors { P = 1 };
#pragma warning disable REFL014
        Assert.That(typeof(Accessors).GetMethod("get_P", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(Accessors).GetMethod("get_P", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)?.Invoke(instance, null), Is.EqualTo(1));

        Assert.That(typeof(Accessors).GetMethod("set_P", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(Accessors).GetMethod("set_P", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)?.Invoke(instance, new object[] { 1 }), Is.Null);

        Assert.That(typeof(Accessors).GetMethod("add_E", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(Accessors).GetMethod("add_E", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)?.Invoke(instance, new object[] { new EventHandler((_, __) => { }) }), Is.Null);

        Assert.That(typeof(Accessors).GetMethod("remove_E", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(Accessors).GetMethod("remove_E", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)?.Invoke(instance, new object[] { new EventHandler((_, __) => { }) }), Is.Null);
#pragma warning restore REFL014
    }
}
