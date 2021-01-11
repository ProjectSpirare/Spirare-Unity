using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Spirare;
using UnityEngine;
using UnityEngine.TestTools;

public class PomlLoaderTest
{
    private XmlDocument xmlDocument;
    private PomlParser parser = new PomlParser();
    private PomlScene scene;
    private List<PomlElement> elements;

    [SetUp]
    public void SetUp()
    {
        var xml = @"
<poml>
    <scene>
        <element position.x=""1"" position.y = ""-1"" position.z=""0.1""/>
    </scene>
</poml>
";
        xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);

        var result = parser.TryParse(xmlDocument, "", out var poml);
        Assert.IsTrue(result);

        scene = poml.Scene;
        elements = scene.Elements;
    }

    [Test]
    public void TryParse_NoPomlTag_ValidFormat()
    {
        var xml = @"
<scene>
    <element/>
    <model/>
</scene>
";
        xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);

        var result = parser.TryParse(xmlDocument, "", out var poml);

        Assert.IsTrue(result);

        var scene = poml.Scene;
        var elements = scene.Elements;
        Assert.AreEqual(2, elements.Count);

        Assert.AreEqual(PomlElementType.Element, elements[0].ElementType);
        Assert.AreEqual(PomlElementType.Model, elements[1].ElementType);
    }

    [Test]
    public void TryParse_PositionAttribute()
    {
        Assert.AreEqual(new Vector3(1, -1, 0.1f), elements[0].Position);
    }
}
