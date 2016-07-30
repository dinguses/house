using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public class State : CollectionMember
{
    public const string membername = "state";
    public State(XElement self) : base(self, membername)
    {
    }

    public string Img
    {
        get
        {
            return this.GetFirst("img", this.Attr, this.Elt);
        }
    }

    public string Description
    {
        get
        {
            return this.GetFirst("desc", this.Attr, this.Elt, this.Val);
        }
    }

    public bool Default
    {
        get
        {
            var def = this.Attr("default");
            return def != null;
        }
    }

}