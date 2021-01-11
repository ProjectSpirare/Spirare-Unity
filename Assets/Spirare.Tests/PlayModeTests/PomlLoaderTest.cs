using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Spirare;
using UnityEngine;
using UnityEngine.TestTools;

public class PomlLoaderTest
{
    private PomlParser parser = new PomlParser();
    /*
    private XmlDocument xmlDocument;
    private PomlScene scene;
    private List<PomlElement> elements;
    */

    /*
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
    */

    private void Parse(string xml, string basePath,
        out Poml poml, out PomlScene scene, out List<PomlElement> elements)
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);

        var result = parser.TryParse(xmlDocument, basePath, out poml);
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
        Parse(xml, "", out var poml, out var scene, out var elements);
        Assert.AreEqual(2, elements.Count);

        Assert.AreEqual(PomlElementType.Element, elements[0].ElementType);
        Assert.AreEqual(PomlElementType.Model, elements[1].ElementType);
    }

    [Test]
    public void TryParse_PositionAttribute()
    {
        var xml = @"
<poml>
    <scene>
        <element position.x=""1"" position.y = ""-1"" position.z=""0.1""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements);
        Assert.AreEqual(new Vector3(1, -1, 0.1f), elements[0].Position);
    }

    [Test]
    public void TryParse_SrcAttribute()
    {
        var xml = @"
<poml>
    <scene>
        <script src=""test.wasm""/>
        <script src=""./test.wasm""/>
        <script src=""dir/test.wasm""/>
        <script src=""./dir/test.wasm""/>
        <script src=""http://example.com/test.wasm""/>
    </scene>
</poml>
";

        var baseUrl = "http://example.net";
        Parse(xml, baseUrl, out var poml, out var scene, out var elements);
        Assert.AreEqual($"{baseUrl}/test.wasm", elements[0].Src);
        Assert.AreEqual($"{baseUrl}/test.wasm", elements[1].Src);
        Assert.AreEqual($"{baseUrl}/dir/test.wasm", elements[2].Src);
        Assert.AreEqual($"{baseUrl}/dir/test.wasm", elements[3].Src);
        Assert.AreEqual("http://example.com/test.wasm", elements[4].Src);
    }

}
