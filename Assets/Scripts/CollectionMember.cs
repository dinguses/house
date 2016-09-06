using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

/// <summary>
/// A CollectionMember wraps a member of a collection.
/// </summary>
public class CollectionMember : Element
{
    /// <summary>
    /// What the node name of a collection member would be
    /// </summary>
    public readonly string membertype;

    public CollectionMember(XElement self, string membertype) : base(self)
    {
        this.membertype = membertype;
    }

    /// <summary>
    /// Get the name of this member.
    /// Collection members have a special shortcut, where they
    /// may put their own name in the node name.
    /// </summary>
    public override string Name
    {
        get
        {
            var eltname = self.Name.ToString();
            if (eltname == membertype) return base.Name;

            return eltname;
        }
    }
}
