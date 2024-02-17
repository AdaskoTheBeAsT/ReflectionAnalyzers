// ReSharper disable All
namespace ValidCode;

using System;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;

public class ConstructorInfoInvoke
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(ConstructorInfoInvoke).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)?.Invoke(null), Is.Not.Null);
        Assert.That(typeof(ConstructorInfoInvoke).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int) }, null)?.Invoke(new object[] { 1 }), Is.Not.Null);

        var type = typeof(ConstructorInfoInvoke);
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        var instance = FormatterServices.GetUninitializedObject(type);
#pragma warning restore SYSLIB0050 // Type or member is obsolete
        Assert.That(type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)?.Invoke(instance, null), Is.Null);
        Assert.That(type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int) }, null)?.Invoke(instance, new object[] { 1 }), Is.Null);
    }

    public ConstructorInfoInvoke()
    {
    }

    public ConstructorInfoInvoke(int value)
    {
    }
}
