using System;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public static class XElementExtensions
{
    /// <summary>
    /// Get the attribute named name, or null if there is none.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Attr(this XElement self, string name)
    {
        var attr = self.Attribute(name);
        if (attr != null) return attr.Value;
        return null;
    }

    /// <summary>
    /// Get a bound method to run Attr.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Func<string> AttrSearch(this XElement self, string name)
    {
        return () => self.Attr(name);
    }

    /// <summary>
    /// Get an enumerable over a single attr of name, or none if there is none.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IEnumerable<string> Attrs(this XElement self, string name)
    {
        return self.Attributes(name).Select((x) => x.Value);
    }

    /// <summary>
    /// Get a bound method to run Attrs.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Func<IEnumerable<string>> AttrsSearch(this XElement self, string name)
    {
        return () => self.Attrs(name);
    }

    /// <summary>
    /// Get the value of the node if it has no children.
    /// A string argument is accepted but ignored for ease of use with
    /// other compositional functions.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Val(this XElement self, string name = null)
    {
        if (!self.HasElements) return self.Value;
        return null;
    }

    /// <summary>
    /// Get a bound method to run Val.
    ///  A string argument is accepted but ignored for ease of use with
    /// other compositional functions.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Func<string> ValSearch(this XElement self, string name = null)
    {
        return () => self.Val(name);
    }

    /// <summary>
    /// Get an enumerator over the value of the node if it is childless,
    /// or an empty enumerator otherwise.
    ///  A string argument is accepted but ignored for ease of use with
    /// other compositional functions.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IEnumerable<string> Vals(this XElement self, string name)
    {
        var val = self.Val(name);
        if (val != null) return new string[] { val };

        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Get a bonud method to run Vals.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Func<IEnumerable<string>> ValsSearch(this XElement self, string name)
    {
        return () => self.Vals(name);
    }

    /// <summary>
    /// Get the first element of the given name,
    /// or null if there are none.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Elt(this XElement self, string name)
    {
        var elt = self.Element(name);

        if (elt != null) return elt.Value;

        return null;
    }

    /// <summary>
    /// Get a bound function to run Elt.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Func<string> EltSearch(this XElement self, string name)
    {
        return () => self.Elt(name);
    }

    /// <summary>
    /// Get an iterator over the elements named name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IEnumerable<string> Elts(this XElement self, string name)
    {
        return self.Elements(name).Cast<string>();
    }

    /// <summary>
    /// Get a bound function to run Elts.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Func<IEnumerable<string>> EltsSearch(this XElement self, string name)
    {
        return () => self.Elts(name);
    }

    /// <summary>
    /// Get a a string of the values of each element named name,
    /// joined by a newline.
    /// 
    /// There will be no trailing newline.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string MultiElts(this XElement self, string name)
    {
        return self.MultiElts(name, "\n");
    }

    /// <summary>
    /// Get a a string of the values of each element named name,
    /// joined by the given seprator.
    /// 
    /// There will be no trailing separator.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string MultiElts(this XElement self, string name, string separator)
    {
        var elts = self.Elts(name);

        if (elts.Count() == 0) return null;

        var sb = new StringBuilder(elts.Count());

        sb.Append(elts.First());

        foreach (var str in elts.Skip(1))
        {
            sb.Append(separator);
            sb.Append(str);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Get a bound function to run MultiElts.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static Func<string> MultiEltsSearch(this XElement self, string name, string separator)
    {
        return () => self.MultiElts(name, separator).ToString();
    }

    /// <summary>
    /// Get a bound function to run MultiElts with a newline
    /// as the separator.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Func<string> MultiEltsSearch(this XElement self, string name)
    {
        return self.MultiEltsSearch(name, "\n");
    }

    /// <summary>
    /// Run each function argument, returning the first non-null result.
    /// </summary>
    /// <param name="searchers"></param>
    /// <returns></returns>
    public static string GetFirst(this XElement self, params Func<string>[] searchers)
    {
        return searchers.Select((x) => x()).FirstOrDefault((x) => x != null);
    }

    /// <summary>
    /// Run each function argument with the argument name, returning the first non-null result.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="searchers"></param>
    /// <returns></returns>
    public static string GetFirst(this XElement self, string name, params Func<string, string>[] searchers)
    {
        return searchers.Select((x) => x(name)).FirstOrDefault((x) => x != null);
    }
}