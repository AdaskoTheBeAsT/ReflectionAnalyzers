namespace ValidCode;

using System.Reflection;
using NUnit.Framework;

public class Indexer
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(Indexer).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(Indexer).GetProperty("Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, typeof(int), new[] { typeof(int) }, null), Is.Not.Null);
    }

    public int this[int i] => 0;
}
