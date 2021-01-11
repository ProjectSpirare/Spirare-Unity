using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using Spirare;
using UnityEngine;
using UnityEngine.TestTools;
using static Spirare.PomlElement;

public class PomlParserTest
{
    private PomlParser parser = new PomlParser();

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

    [Test]
    public void TryParse_ArgsAttribute()
    {
        var xml = @"
<poml>
    <scene>
        <script args=""--position -1.0 2.0 3.0""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements);
        var element = elements[0] as PomlScriptElement;
        Assert.IsNotNull(element);
        Assert.AreEqual(4, element.Args.Count);

        var args = element.Args;
        Assert.AreEqual("--position", args[0]);
        Assert.AreEqual("-1.0", args[1]);
        Assert.AreEqual("2.0", args[2]);
        Assert.AreEqual("3.0", args[3]);
    }

    [Test]
    public void TryParse_PrimitiveAttribute()
    {
        var xml = @"
<poml>
    <scene>
        <primitive type=""cube""/>
        <primitive type=""plane""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements);
        var element0 = elements[0] as PomlPrimitiveElement;
        Assert.IsNotNull(element0);
        Assert.AreEqual(PomlPrimitiveElement.PomlPrimitiveElementType.Cube, element0.PrimitiveType);

        var element1 = elements[1] as PomlPrimitiveElement;
        Assert.IsNotNull(element1);
        Assert.AreEqual(PomlPrimitiveElement.PomlPrimitiveElementType.Plane, element1.PrimitiveType);
    }
}
