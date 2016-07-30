using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public struct State
{
    public readonly XElement self;

    public State(XElement self)
    {
        if (self == null) throw new System.Exception();

        this.self = self;
    }

    public string Img
    {
        get
        {
            return self.AttributeOrElement("img").FirstOrDefault();
        }
    }

    public string Name
    {
        get
        {
            return self.NodeNameOrAttrOrValue("state");
        }
    }

    public string Description
    {
        get
        {
            return self.AttributeOrElement("desc", "description", "text").FirstOrDefault();
        }
    }

    public static implicit operator XElement(State s)
    {
        return s.self;
    }

    public static implicit operator string(State s)
    {
        return s.Name;
    }
}