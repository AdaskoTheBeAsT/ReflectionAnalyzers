namespace ValidCode;

using System.Reflection;
using NUnit.Framework;

public class WhenTypeIsNotInSource
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(string).GetField(nameof(string.Empty), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly), Is.Not.Null);

        Assert.That(typeof(string).GetMethod(nameof(string.Contains), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(string) }, null), Is.Not.Null);

        Assert.That(typeof(string).GetProperty(nameof(string.Length), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
    }
}
