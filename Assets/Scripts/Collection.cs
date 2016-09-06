using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Collections;

/// <summary>
/// A collection wraps a list/set of Elements.
/// In a collection, each member MUST be a member of the collection.
/// They may not all have the same membertype name, as a shortcut.
/// </summary>
public class Collection : Element, IEnumerable<CollectionMember>
{
    /// <summary>
    /// The collectiontype is the name of the collection element.
    /// e.g. for a list of rooms, the collection type would be "rooms"
    /// </summary>
    public readonly string collectiontype;
    /// <summary>
    /// The membertype is the name of each member element.
    /// e.g. for a list of rooms, the collection type would be "room"
    /// </summary>
    public readonly string membertype;

    /// <summary>
    /// A Collection's collectionname *MUST* match the name of the node.
    /// </summary>
    /// <param name="self">The xelement to wrap./</param>
    /// <param name="collectiontype">The name of the collection element</param>
    /// <param name="membertype">The name of the member element</param>
    public Collection(XElement self, string collectiontype, string membertype) : base(self)
    {
        if (self.Name.ToString() != collectiontype) throw new Exception();

        if (membertype == null) throw new Exception();

        this.collectiontype = collectiontype;
        this.membertype = membertype;
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