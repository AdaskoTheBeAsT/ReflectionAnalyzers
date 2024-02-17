// ReSharper disable All
namespace ValidCode;

using System.Reflection;
using NUnit.Framework;

public class GetNestedType
{
    [Test]
    public void Valid()
    {
        Assert.That(GetType().GetNestedType(nameof(PublicStatic), BindingFlags.Public), Is.Not.Null);
        Assert.That(this.GetType().GetNestedType(nameof(PublicStatic), BindingFlags.Public), Is.Not.Null);
        Assert.That(this?.GetType().GetNestedType(nameof(PublicStatic), BindingFlags.Public), Is.Not.Null);
        Assert.That(typeof(GetNestedType).GetNestedType(nameof(PublicStatic), BindingFlags.Public), Is.Not.Null);
        Assert.That(typeof(GetNestedType).GetNestedType(nameof(Public), BindingFlags.Public), Is.Not.Null);
        Assert.That(typeof(GetNestedType).GetNestedType(nameof(PrivateStatic), BindingFlags.NonPublic), Is.Not.Null);
        Assert.That(typeof(GetNestedType).GetNestedType(nameof(Private), BindingFlags.NonPublic), Is.Not.Null);
    }

    public static class PublicStatic
    {
    }

    public class Public
    {
    }

    private static class PrivateStatic
    {
    }

    private class Private
    {
    }
}
