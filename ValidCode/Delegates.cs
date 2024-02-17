namespace ValidCode;

using System;
using System.Reflection;
using NUnit.Framework;

public class Delegates
{
    [Test]
    public void Valid()
    {
#pragma warning disable REFL039 // Prefer typeof(...) over instance.GetType when the type is sealed.
        var @delegate = Create();
        Action action = () => { };
        Action<int> actionInt = _ => { };
        Func<int> funcInt = () => 0;
        Func<int, int> funcIntInt = x => x;
        Assert.That(@delegate.GetType().GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(action.GetType().GetMethod(nameof(action.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(action.GetType().GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);

        Assert.That(actionInt.GetType().GetMethod(nameof(actionInt.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(actionInt.GetType().GetMethod(nameof(Action<int>.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);

        Assert.That(funcInt.GetType().GetMethod(nameof(funcInt.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(funcInt.GetType().GetMethod(nameof(Func<int>.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);

        Assert.That(funcIntInt.GetType().GetMethod(nameof(funcIntInt.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(funcIntInt.GetType().GetMethod(nameof(Func<int, int>.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
#pragma warning restore REFL039 // Prefer typeof(...) over instance.GetType when the type is sealed.

        Assert.That(typeof(Action).GetMethod(nameof(action.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(typeof(Action).GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);

        Assert.That(typeof(Action<int>).GetMethod(nameof(actionInt.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(typeof(Action<int>).GetMethod(nameof(Action<int>.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);

        Assert.That(typeof(Func<int>).GetMethod(nameof(funcInt.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);
        Assert.That(typeof(Func<int>).GetMethod(nameof(Func<int>.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null), Is.Not.Null);

        Assert.That(typeof(Func<int, int>).GetMethod(nameof(funcIntInt.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
        Assert.That(typeof(Func<int, int>).GetMethod(nameof(Func<int, int>.Invoke), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, new[] { typeof(int) }, null), Is.Not.Null);
    }

    private static Delegate Create() => new Action(delegate { });
}
