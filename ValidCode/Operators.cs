// ReSharper disable All
namespace ValidCode;

using System.Reflection;
using NUnit.Framework;

public class Operators
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(Operators).GetMethod("op_Addition",             BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(Operators), typeof(Operators) }, null)!.Invoke(null, new object?[] { null, null }), Is.EqualTo(1));
        Assert.That(typeof(Operators).GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(Operators), typeof(Operators) }, null)!.Invoke(null, new object?[] { null, null }), Is.True);
        Assert.That(typeof(Operators).GetMethod("op_Inequality", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(Operators), typeof(Operators) }, null)!.Invoke(null, new object?[] { null, null }), Is.True);
        Assert.That(typeof(Operators).GetMethod("op_Explicit",   BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(Operators) },                    null)!.Invoke(null, new object?[] { (Operators?)null }), Is.EqualTo(2));
        Assert.That(typeof(Operators).GetMethod("op_Explicit", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null)!.Invoke(null, new object[] { 1 }), Is.Null);
    }

    public static int operator +(Operators left, Operators right) => 1;

    public static bool operator ==(Operators left, Operators right) => true;

    public static bool operator !=(Operators left, Operators right) => true;

    public static explicit operator int(Operators c) => 2;

    public static explicit operator Operators?(int c) => null;

    protected bool Equals(Operators other)
    {
        throw new System.NotImplementedException();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((Operators)obj);
    }

    public override int GetHashCode() => 1;
}
