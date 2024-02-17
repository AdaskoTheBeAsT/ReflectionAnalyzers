namespace ValidCode;

using System;
using System.Reflection;
using NUnit.Framework;

public interface IExplicitImplicit
{
    event EventHandler? E;
}

public class ExplicitImplicit : IExplicitImplicit
{
    [Test]
    public void Valid()
    {
        Assert.That(typeof(ExplicitImplicit).GetEvent(nameof(this.Bar), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
        Assert.That(typeof(IExplicitImplicit).GetEvent(nameof(IExplicitImplicit.E), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly), Is.Not.Null);
    }

    internal event EventHandler? Bar;

    event EventHandler? IExplicitImplicit.E
    {
        add => this.Bar += value;
        remove => this.Bar -= value;
    }
}
