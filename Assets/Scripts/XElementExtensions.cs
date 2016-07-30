using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public static class XElementExtensions
{
    /// <summary>
    /// Returns an Enumerable of strings. First the the attribute value if it exists,
    /// followed by all child elements.
    /// It's up to you to decide what should take priority. The LINQ methods FirstOrDefault()
    /// and/or ElementAtDefault() should be helpful.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IEnumerable<string> AttributeOrElement(this XElement self, string name, params string[] names)
    {
        IEnumerable<string> strings = Enumerable.Empty<string>();

        foreach (var str in new string[] { name }.Concat(names))
        {
            strings = strings.Concat(self.Attributes(str).Select((x) => x.Value))
                .Concat(self.Elements(str).Select((x) => x.Value));
        }

        return strings;
    }

    /// <summary>
    /// Returns the value of the name attribute if it exists, or falls back
    /// to the content of the element.
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string NameOrValue(this XElement self)
    {
        var attr = self.Attribute("name");
        if (attr != null) return attr.Value;

        return self.Value;
    }

    /// <summary>
    /// Returns the name of the node (unless it matches the ignore value),
    /// falling back to a name attribute or the element value.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="ignore">The value to match against.</param>
    /// <returns></returns>
    public static string NodeNameOrAttrOrValue(this XElement self, string ignore)
    {
        var name = self.Name.ToString();
        if (name != ignore) return name;

        return self.NameOrValue();
    }
}