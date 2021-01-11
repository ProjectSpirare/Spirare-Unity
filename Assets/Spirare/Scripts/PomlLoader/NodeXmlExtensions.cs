using System.Xml;

namespace Spirare
{
    internal static class XmlNodeExtensions
    {
        public static bool TryGetAttribute(this XmlNode node, string key, out string value)
        {
            var type = node.Attributes[key];
            if (type == null)
            {
                value = null;
                return false;
            }

            value = type.Value;
            return true;
        }

        public static string GetAttribute(this XmlNode node, string key, string defaultValue = "")
        {
            if (node.TryGetAttribute(key, out var value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
