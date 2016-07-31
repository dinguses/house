using System;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public class Element
{
    public readonly XElement self;

    public Element(XElement self)
    {
        if (self == null) throw new System.Exception();

        this.self = self;
    }

    /// <summary>
    /// An element may have a name in an attribute, the first element named "name",
    /// or the value of the element.
    /// 
    /// More specific element types may override this, to change priority
    /// or get the name from other sources.
    /// </summary>
    public virtual string Name
    {
        get
        {
            return self.GetFirst("name", self.Attr, self.Elt, self.Val);
        }
    }



    public static implicit operator XElement(Element s)
    {
        return s.self;
    }
}
