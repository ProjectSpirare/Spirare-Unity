using System;
using System.IO;

namespace Spirare
{
    internal static class FilePathUtility
    {
        public static string GetAbsolutePath(string path, string basePath)
        {
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                // not a valid URI
                return path;
            }
            if (uri.IsAbsoluteUri)
            {
                return path;
            }

            if (!Uri.TryCreate(basePath, UriKind.RelativeOrAbsolute, out Uri baseUri))
            {
                // not a valid URI
                return path;
            }

            try
            {
                if (baseUri.IsFile)
                {
                    UnityEngine.Debug.Log("file");
                    return Path.Combine(basePath, "..", path);
                }
                else
                {
                    /*
                    var relative = path.StartsWith(".");

                    if (relative)
                    {
                        return CombineUri(baseUri, path);
                    }
                    else
                    {
                        UnityEngine.Debug.Log(baseUri.Authority);
                        var rootPath = baseUri.GetLeftPart(UriPartial.Authority);
                        UnityEngine.Debug.Log(rootPath);
                        UnityEngine.Debug.Log(path);
                        return CombineUri(baseUri, path);
                    }
                    */
                    return CombineUri(baseUri, path);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning(e);
                return "";
            }
        }

        private static string CombineUri(Uri baseUri, string relativeUri)
        {
            var relative = relativeUri.StartsWith(".");
            try
            {
                Uri uri;
                if (relative)
                {
                    uri = new Uri(baseUri, relativeUri);
                }
                else
                {
                    var rootPath = baseUri.GetLeftPart(UriPartial.Authority);
                    var rootUrl = new Uri(rootPath);
                    uri = new Uri(rootUrl, relativeUri);
                }
                return uri.AbsoluteUri;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
