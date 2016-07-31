using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Collections;

public class Collection : Element, IEnumerable<CollectionMember>
{
    public readonly string collectiontype;
    public readonly string membertype;

    /// <summary>
    /// A Collection's collectionname *MUST* match the name of the node.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="collectiontype"></param>
    /// <param name="membertype"></param>
    public Collection(XElement self, string collectiontype, string membertype) : base(self)
    {
        if (self.Name.ToString() != collectiontype) throw new Exception();

        if (membertype == null) throw new Exception();

        this.collectiontype = collectiontype;
    }

    /// <summary>
    /// A Collection may have a custom name attribute but otherwise is just the given
    /// collection name.
    /// </summary>
    public override string Name
    {
        get
        {
            return self.GetFirst(self.AttrSearch("name"), () => collectiontype);
        }
    }

    public virtual IEnumerator<CollectionMember> GetEnumerator()
    {
        return self.Elements().Select((x) => new CollectionMember(x, membertype)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}