using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Spirare;
using UnityEngine;
using UnityEngine.TestTools;

public class FilePathUtilityTests
{
    private readonly static string httpUrl = "http://example.com";

    [Test]
    public void GetAbsolutePath_HttpRootRelative()
    {
        string path;

        var basePathList = new string[]
        {
            "http://example.com",
            "http://example.com/",
            "http://example.com/index.html",
            "http://example.com/sub/",
            "http://example.com/sub/index.html",
        };

        foreach (var basePath in basePathList)
        {

            path = FilePathUtility.GetAbsolutePath("path", basePath);
            Assert.AreEqual($"{httpUrl}/path", path);

            path = FilePathUtility.GetAbsolutePath("/path", basePath);
            Assert.AreEqual($"{httpUrl}/path", path);
        }
    }

    [Test]
    public void GetAbsolutePath_HttpRelative()
    {
        string path;

        var basePathList = new string[]
        {
            "http://example.com",
            "http://example.com/",
            "http://example.com/index.html",
        };

        foreach (var basePath in basePathList)
        {

            path = FilePathUtility.GetAbsolutePath("./path", basePath);
            Assert.AreEqual($"{httpUrl}/path", path);
        }

        basePathList = new string[]
        {
            "http://example.com/sub/",
            "http://example.com/sub/index.html",
        };

        foreach (var basePath in basePathList)
        {
            path = FilePathUtility.GetAbsolutePath("./path", basePath);
            Assert.AreEqual($"{httpUrl}/sub/path", path);
        }
    }

    [Test]
    public void GetAbsolutePath_HttpAbsolute()
    {
        string path;

        var basePathList = new string[]
        {
            "http://example.com",
            "http://example.com/",
            "http://example.com/index.html",
            "http://example.com/sub/",
            "http://example.com/sub/index.html",
        };

        foreach (var basePath in basePathList)
        {
            path = FilePathUtility.GetAbsolutePath("http://example.net/path", basePath);
            Assert.AreEqual("http://example.net/path", path);
        }
    }

    [Test]
    public void GetAbsolutePath_FileRelative()
    {
        string path;
        var basePath = @"C:\foo\bar\foobar.xml";

        path = FilePathUtility.GetAbsolutePath("path", basePath);
        Assert.AreEqual(@"C:\foo\bar\path", path);

        path = FilePathUtility.GetAbsolutePath("C:\\path", basePath);
        Assert.AreEqual(@"C:\path", path);

        path = FilePathUtility.GetAbsolutePath("./path", basePath);
        Assert.AreEqual(@"C:\foo\bar\path", path);
    }

}
