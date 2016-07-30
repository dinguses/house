using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public class State : CollectionMember
{
    public State(XElement self) : base(self, "state")
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

}