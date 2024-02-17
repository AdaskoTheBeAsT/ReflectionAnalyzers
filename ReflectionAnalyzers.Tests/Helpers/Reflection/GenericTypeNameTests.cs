#pragma warning disable CS8629 // Nullable value type may be null.
namespace ReflectionAnalyzers.Tests.Helpers.Reflection;

using System.Linq;
using NUnit.Framework;

public static class GenericTypeNameTests
{
    [TestCase("System.Int32")]
    [TestCase("System.Nullable`1")]
    [TestCase("System.Nullable`1[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]")]
    [TestCase("System.Nullable`1 [System.Int32]")]
    [TestCase("System.Collections.Generic.KeyValuePair`2 [System.Int32,System.String]")]
    public static void TryGetGenericWhenFalse(string name)
    {
        Assert.That(GenericTypeName.TryParse(name), Is.Null);
    }

    [TestCase("System.Nullable`1[System.Int32]", "System.Int32")]
    [TestCase("System.Nullable`1[[System.Int32]]", "System.Int32")]
    [TestCase("System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", "System.Int32")]
    public static void TryGetGenericWhenNullable(string name, string arg)
    {
        var generic = GenericTypeName.TryParse(name).Value;
        Assert.That(generic.MetadataName, Is.EqualTo("System.Nullable`1"));
        var typeArgument = generic.TypeArguments.Single();
        Assert.That(typeArgument.MetadataName, Is.EqualTo(arg));
        Assert.That(typeArgument.TypeArguments, Is.Null);
    }

    [TestCase("System.Collections.Generic.KeyValuePair`2[System.Int32,System.String]", "System.Int32", "System.String")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[System.Int32, System.String]", "System.Int32", "System.String")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[System.Int32,  System.String]", "System.Int32", "System.String")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[ System.Int32,System.String]", "System.Int32", "System.String")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[ System.Int32, System.String]", "System.Int32", "System.String")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[System.Int32,System.String ]", "System.Int32", "System.String ")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[System.Int32 ,System.String]", "System.Int32 ", "System.String")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[ System.Int32, System.String ]", "System.Int32", "System.String ")]
    [TestCase("System.Collections.Generic.KeyValuePair`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", "System.Int32", "System.String")]
    public static void TryGetGenericWhenKeyValuePair(string name, string arg0, string arg1)
    {
        var generic = GenericTypeName.TryParse(name).Value;
        Assert.That(generic.MetadataName, Is.EqualTo("System.Collections.Generic.KeyValuePair`2"));
        var typeArguments = generic.TypeArguments;
        Assert.That(typeArguments.Length, Is.EqualTo(2));
        Assert.That(typeArguments[0].MetadataName, Is.EqualTo(arg0));
        Assert.That(typeArguments[0].TypeArguments, Is.Null);
        Assert.That(typeArguments[1].MetadataName, Is.EqualTo(arg1));
        Assert.That(typeArguments[1].TypeArguments, Is.Null);
    }

    [TestCase("System.Nullable`1[System.Collections.Generic.KeyValuePair`2[System.Int32,System.String]]", "System.Int32", "System.String")]
    [TestCase("System.Nullable`1[[System.Collections.Generic.KeyValuePair`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", "System.Int32", "System.String")]
    public static void TryGetGenericWhenNullableKeyValuePair(string name, string arg0, string arg1)
    {
        var generic = GenericTypeName.TryParse(name).Value;
        Assert.That(generic.MetadataName, Is.EqualTo("System.Nullable`1"));
        var typeArgument = generic.TypeArguments.Single();
        Assert.That(typeArgument.MetadataName, Is.EqualTo("System.Collections.Generic.KeyValuePair`2"));
        var genericArguments = typeArgument.TypeArguments;
        Assert.That(genericArguments.Count, Is.EqualTo(2));
        Assert.That(genericArguments[0].MetadataName, Is.EqualTo(arg0));
        Assert.That(genericArguments[0].TypeArguments, Is.Null);
        Assert.That(genericArguments[1].MetadataName, Is.EqualTo(arg1));
        Assert.That(genericArguments[1].TypeArguments, Is.Null);
    }
}
