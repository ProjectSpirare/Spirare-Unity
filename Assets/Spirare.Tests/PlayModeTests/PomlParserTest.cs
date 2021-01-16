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
        out Poml poml,
        out PomlScene scene, out List<PomlElement> elements,
        out PomlResource resource, out List<PomlElement> resourceElements)
    {
        var result = parser.TryParse(xml, basePath, out poml);
        Assert.IsTrue(result);

        scene = poml.Scene;
        elements = scene.Elements;

        resource = poml.Resource;
        resourceElements = resource.Elements;
    }

    [Test]
    public void TryParse_NoPomlTag_ValidFormat()
    {
        var xml = @"
<scene>
    <element/>
    <model/>
</scene>
<resource>
    <element/>
</resource>
";
        Parse(xml, "", out var poml, out var scene, out var elements, out _, out var resourceElements);
        Assert.AreEqual(2, elements.Count);

        Assert.AreEqual(PomlElementType.Element, elements[0].ElementType);
        Assert.AreEqual(PomlElementType.Model, elements[1].ElementType);

        Assert.AreEqual(1, resourceElements.Count);
    }

    [Test]
    public void TryParse_PositionAttribute()
    {
        var xml = @"
<poml>
    <scene>
        <element position=""1 -2 0.1""/>
        <element position=""1, -2, 0.1""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements, out _, out _);
        Assert.AreEqual(new Vector3(2, 0.1f, 1f), elements[0].Position);
        Assert.AreEqual(new Vector3(2, 0.1f, 1f), elements[1].Position);
    }

    [Test]
    public void TryParse_ScaleAttribute()
    {
        var xml = @"
<poml>
    <scene>
        <element/>
        <element scale=""1 2 0.1""/>
        <element scale=""1, 2, 0.1""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements, out _, out _);
        Assert.AreEqual(new Vector3(1, 1, 1), elements[0].Scale);
        Assert.AreEqual(new Vector3(2, 0.1f, 1f), elements[1].Scale);
        Assert.AreEqual(new Vector3(2, 0.1f, 1f), elements[2].Scale);
    }

    [Test]
    public void TryParse_RotationAttribute()
    {
        var xml = @"
<poml>
    <scene>
        <element/>
        <element rotation=""0 0 0 1""/>
        <element rotation=""0, 0, 0, 1""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements, out _, out _);
        Assert.AreEqual(new Quaternion(0, 0, 0, 1), elements[0].Rotation);
        Assert.AreEqual(new Quaternion(0, 0, 0, -1), elements[1].Rotation);
        Assert.AreEqual(new Quaternion(0, 0, 0, -1), elements[2].Rotation);
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
        Parse(xml, baseUrl, out var poml, out var scene, out var elements, out _, out _);
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

        Parse(xml, "", out var poml, out var scene, out var elements, out _, out _);
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
    public void TryParse_PrimitiveShape()
    {
        var xml = @"
<poml>
    <scene>
        <primitive shape=""cube""/>
        <primitive shape=""plane""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements, out _, out _);
        var element0 = elements[0] as PomlPrimitiveElement;
        Assert.IsNotNull(element0);
        Assert.AreEqual(PomlPrimitiveElement.PomlPrimitiveElementType.Cube, element0.PrimitiveType);

        var element1 = elements[1] as PomlPrimitiveElement;
        Assert.IsNotNull(element1);
        Assert.AreEqual(PomlPrimitiveElement.PomlPrimitiveElementType.Plane, element1.PrimitiveType);
    }

    [Test]
    public void TryParse_Attribute()
    {
        var xml = @"
<poml>
    <scene>
        <element />
        <element attribute=""static""/>
        <element attribute=""equipable""/>
        <element attribute=""static equipable""/>
    </scene>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements, out _, out _);
        Assert.AreEqual(ElementAttributeType.None, elements[0].Attribute);
        Assert.AreEqual(ElementAttributeType.Static, elements[1].Attribute);
        Assert.AreEqual(ElementAttributeType.Equipable, elements[2].Attribute);
        Assert.AreEqual(ElementAttributeType.Static | ElementAttributeType.Equipable, elements[3].Attribute);
    }


    [Test]
    public void TryParse_Resource()
    {
        var xml = @"
<poml>
    <scene>
        <primitive type=""cube""/>
    </scene>
    <resource>
        <primitive id=""a"" type=""cube""/>
        <primitive id=""b"" type=""plane""/>
    </resource>
</poml>
";

        Parse(xml, "", out var poml, out var scene, out var elements, out var resource, out var resourceElements);
        Assert.AreEqual(2, resourceElements.Count);
        var resource0 = resourceElements[0];
        Assert.AreEqual("a", resource0.Id);

        var resource1 = resourceElements[1];
        Assert.AreEqual("b", resource1.Id);
    }
}
