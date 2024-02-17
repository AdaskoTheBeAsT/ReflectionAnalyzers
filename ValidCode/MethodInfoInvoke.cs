namespace ValidCode;

using System.Reflection;
using NUnit.Framework;

public class MethodInfoInvoke
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(MethodInfoInvoke).GetMethod(nameof(Id),         System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null)!.Invoke(null, new object[] { 1 }),             Is.EqualTo(1));
        Assert.That(typeof(MethodInfoInvoke).GetMethod(nameof(IdOptional), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null)!.Invoke(null, new object[] { Missing.Value }), Is.EqualTo(2));
        Assert.That(typeof(MethodInfoInvoke).GetMethod(nameof(IdOptional), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null)!.Invoke(null, new object[] { 3 }),             Is.EqualTo(3));
    }

    public static int Id(int i) => i;

    public static int IdOptional(int i = 2) => i;
}
