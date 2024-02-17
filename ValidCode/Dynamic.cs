namespace ValidCode;

using System.Collections.Generic;
using System.Dynamic;
using NUnit.Framework;

public class Dynamic
{
    [Test]
    public void Index()
    {
        dynamic expando = new ExpandoObject();
        expando.name = "John";
        Assert.That(((IDictionary<string, object>)expando)["name"], Is.EqualTo("John"));
    }
}
