using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;

public class StateSet : Collection, IEnumerable<State>
{
    public StateSet(XElement self) : base(self, "states", State.membername)
    {
    }

    IEnumerator<State> IEnumerable<State>.GetEnumerator()
    {
        return self.Elements().Select((x) => new State(x)).GetEnumerator();
    }

    public string Default
    {
        get
        {
            var attr = this.Attr("default");

            if (attr != null) return attr;

            var states = this as IEnumerable<State>;

            //attr = states.FirstOrDefault((x) => x.Default);
            return null;
        }
    }
}