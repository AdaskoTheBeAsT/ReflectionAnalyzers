// ReSharper disable All
namespace ValidCode;

using System.Reflection;
using NUnit.Framework;

public class GenericMember
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(GenericMember).GetMethod(nameof(this.Id), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(GenericMember).GetNestedType("Bar`1", BindingFlags.Public), Is.Not.Null);
    }

    public T Id<T>(T value) => value;

    public class Bar<T>
    {
    }
}
