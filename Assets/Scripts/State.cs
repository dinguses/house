﻿using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public class State : CollectionMember
{
    public const string membername = "state";
    public State(XElement self) : base(self, membername)
    {
    }

    /// <summary>
    /// Checks for an img attribute, then element
    /// </summary>
    public string Img
    {
        get
        {
            return this.GetFirst("img", this.Attr, this.Elt);
        }
    }

    /// <summary>
    /// Checks for a "desc" as an attribute, then element, then value.
    /// </summary>
    public string Description
    {
        get
        {
            return this.GetFirst("desc", this.Attr, this.Elt, this.Val);
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
            var def = this.Attr("default");
            return def != null;
        }
    }

    public override string ToString()
    {
        return "State " + Name + " (" + Description + "), img " + Img;
    }
}