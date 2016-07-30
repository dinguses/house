using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Collections;

public class Collection : Element, IEnumerable<CollectionMember>
{
    public readonly string collectionname;
    public readonly string membertype;

    public Collection(XElement self, string collectionname, string membertype) : base(self)
    {
        if (self.Name.ToString() != collectionname) throw new Exception();

        if (membertype == null) throw new Exception();

        this.collectionname = collectionname;
    }

    public override string Name
    {
        get
        {
            return this.GetFirst(this.AttrSearch("name"), () => collectionname);
        }
    }

    public IEnumerator<CollectionMember> GetEnumerator()
    {
        return self.Elements().Select((x) => new CollectionMember(x, membertype)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}