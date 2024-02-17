namespace ReflectionAnalyzers.Tests.Helpers.Reflection;

using System.Linq;
using NUnit.Framework;

public static class GenericTypeArgumentTests
{
    [TestCase("[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]")]
    public static void TryParseInvalid(string text)
    {
        Assert.That(GenericTypeArgument.TryParseBracketedList(text, 0, 1, out _), Is.False);
    }

    [TestCase("[System.Int32]",                                                                                 "System.Int32")]
    [TestCase("[ System.Int32]",                                                                                "System.Int32")]
    [TestCase("[System.Int32 ]",                                                                                "System.Int32 ")]
    [TestCase("[[System.Int32]]",                                                                               "System.Int32")]
    [TestCase("[ [System.Int32] ]",                                                                             "System.Int32")]
    [TestCase("[[System.Int32 ]]",                                                                              "System.Int32 ")]
    [TestCase("[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]",  "System.Int32")]
    [TestCase("[[ System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", "System.Int32")]
    [TestCase("[[System.Int32 , mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", "System.Int32 ")]
    public static void TryParseSingle(string name, string arg)
    {
        Assert.That(GenericTypeArgument.TryParseBracketedList(name, 0, 1, out var args), Is.True);
        var typeArgument = args.Single();
        Assert.That(typeArgument.MetadataName, Is.EqualTo(arg));
        Assert.That(typeArgument.TypeArguments, Is.Null);
    }

    [TestCase("[System.Int32,System.String]",                                                                                                                                                               "System.Int32", "System.String")]
    [TestCase("[System.Int32, System.String]",                                                                                                                                                              "System.Int32", "System.String")]
    [TestCase("[System.Int32,  System.String]",                                                                                                                                                             "System.Int32", "System.String")]
    [TestCase("[ System.Int32,System.String]",                                                                                                                                                              "System.Int32", "System.String")]
    [TestCase("[ System.Int32, System.String]",                                                                                                                                                             "System.Int32", "System.String")]
    [TestCase("[ [System.Int32] , [System.String] ]",                                                                                                                                                       "System.Int32", "System.String")]
    [TestCase("[System.Int32,System.String ]",                                                                                                                                                              "System.Int32", "System.String ")]
    [TestCase("[System.Int32 ,System.String]",                                                                                                                                                              "System.Int32 ", "System.String")]
    [TestCase("[ System.Int32, System.String ]",                                                                                                                                                            "System.Int32", "System.String ")]
    [TestCase("[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", "System.Int32", "System.String")]
    public static void TryGetGenericWhenKeyValuePair(string name, string arg0, string arg1)
    {
        Assert.That(GenericTypeArgument.TryParseBracketedList(name, 0, 2, out var args), Is.True);
        var typeArgument = args[0];
        Assert.That(typeArgument.MetadataName, Is.EqualTo(arg0));
        Assert.That(typeArgument.TypeArguments, Is.Null);
        typeArgument = args[1];
        Assert.That(typeArgument.MetadataName, Is.EqualTo(arg1));
        Assert.That(typeArgument.TypeArguments, Is.Null);
    }
}
