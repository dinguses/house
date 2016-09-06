using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;

/// <summary>
/// A State is a State that something can be it.
/// It is a member of a StateSet.
/// 
/// It adds convenience methods for an image, description, and the "default" field.
/// Note that the default field isn't 100% authoritative-- check the StateSet for that.
/// </summary>
public class State : CollectionMember
{
    public const string membername = "state";
    public State(XElement self) : base(self, membername)
    {
    }

    private string _img;

    /// <summary>
    /// Checks for an img attribute, then element
    /// </summary>
    public string Img
    {
        get
        {
            if (_img == null) _img = self.GetFirst("img", self.Attr, self.Elt);

            return _img;
        }
    }

    private string _desc;
    /// <summary>
    /// Checks for a "desc" as an attribute, then element, then value.
    /// </summary>
    public string Description
    {
        get
        {
           if (_desc == null) _desc = self.GetFirst("desc", self.Attr, self.MultiElts, self.Val);

            return _desc;
        }
    }

    /// <summary>
    /// Does this node have the "default" attribute?
    /// Note that this may be false, but it may still be the default
    /// just because it's first in the collection.
    /// </summary>
    public bool Default
    {
        get
        {
            var def = self.Attr("default");
            return def != null;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder(7);
        sb.Append("State ");
        sb.Append(Name);
        sb.Append(" (");
        sb.Append(Description);
        sb.Append(")");

        if (Img != null)
        {
            sb.Append(", img ");
            sb.Append(Img);
        }

        return sb.ToString();
    }
}