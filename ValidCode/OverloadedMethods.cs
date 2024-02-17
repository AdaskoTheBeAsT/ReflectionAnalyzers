namespace ValidCode;

using System.Reflection;
using NUnit.Framework;

public class OverloadedMethods
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(this.PublicStatic), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(this.PublicStatic), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(double) }, null), Is.Not.Null);

        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(PublicStaticInstance), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,   null, new[] { typeof(int) },    null), Is.Not.Null);
        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(PublicStaticInstance), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(double) }, null), Is.Not.Null);

        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(this.PublicInstance), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(this.PublicInstance), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(double) }, null), Is.Not.Null);

        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(this.PublicPrivateInstance), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(typeof(OverloadedMethods).GetMethod(nameof(this.PublicPrivateInstance), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(double) }, null), Is.Not.Null);
    }

    public static int PublicStatic(int value) => value;

    public static double PublicStatic(double value) => value;

    public static int PublicStaticInstance(int value) => value;

    public double PublicStaticInstance(double value) => value;

    public int PublicInstance(int value) => value;

    public double PublicInstance(double value) => value;

    public int PublicPrivateInstance(int value) => value;

    private double PublicPrivateInstance(double value) => value;
}
