namespace ReflectionAnalyzers.Tests.Helpers.Reflection;

using System;
using System.Linq;

using NUnit.Framework;

public static class BindingFlagsTests
{
    private static readonly System.Reflection.BindingFlags[] SystemReflectionFlags = Enum.GetValues(typeof(System.Reflection.BindingFlags))
                                                                                    .Cast<System.Reflection.BindingFlags>()
                                                                                    .ToArray();

    [TestCaseSource(nameof(SystemReflectionFlags))]
    public static void IsMatched(System.Reflection.BindingFlags system)
    {
        Assert.That(Enum.TryParse<ReflectionAnalyzers.BindingFlags>(system.ToString("G"), out var local), Is.True);
        Assert.That(local.ToString("D"), Is.EqualTo(system.ToString("D")));
    }
}
