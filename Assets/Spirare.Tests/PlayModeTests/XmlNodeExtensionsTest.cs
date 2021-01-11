using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Spirare;
using UnityEngine;
using UnityEngine.TestTools;

public class XmlNodeExtensionsTest
{
    private XmlDocument xmlDocument;

    [SetUp]
    public void SetUp()
    {
        var xml = @"
<element src=""src.wasm""/>
";
        xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
    }

    [Test]
    public void TryGetAttribute_ExistentAttribute()
    {
        var element = xmlDocument.ChildNodes[0];
        var result = element.TryGetAttribute("src", out var src);
        Assert.IsTrue(result);
        Assert.AreEqual("src.wasm", src);
    }

    [Test]
    public void TryGetAttribute_NonExistentAttribute()
    {
        var element = xmlDocument.ChildNodes[0];
        var result = element.TryGetAttribute("non_existent", out var src);
        Assert.IsFalse(result);
    }
    
    [Test]
    public void GetAttribute_ExistentAttribute()
    {
        var element = xmlDocument.ChildNodes[0];
        var src = element.GetAttribute("src");
        Assert.AreEqual("src.wasm", src);
    }

    [Test]
    public void GetAttribute_NonExistentAttribute()
    {
        var element = xmlDocument.ChildNodes[0];
        var src = element.GetAttribute("non_existent", "default.wasm");
        Assert.AreEqual("default.wasm", src);
    }
}
