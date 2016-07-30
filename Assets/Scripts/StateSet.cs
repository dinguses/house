using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Text;
using UnityEngine;

public class StateSet : Collection, IEnumerable<State>
{
    public const string collectionname = "states";
    State[] states;
    public StateSet(XElement self) : base(self, collectionname, State.membername)
    {
        this.states = self.Elements().Select((x) => new State(x)).ToArray();
    }

    IEnumerator<State> IEnumerable<State>.GetEnumerator()
    {
        return states.AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Checks for a default attribute, a default attribute on any member, and finally
    /// falls back to the first.
    /// </summary>
    public State Default
    {
        get
        {
            var attr = this.Attr("default");

            if (attr != null) return states.First((x) => x.Name == attr);

            var withattr = states.FirstOrDefault((x) => x.Default);

            if (withattr != null) return withattr;

            return states.FirstOrDefault();
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (Name == collectionname) sb.Append("States: ");
        else
        {
            sb.Append(Name); sb.Append(" states: ");
        }

        sb.Append("default "); sb.AppendLine(Default.Name);

        foreach (var state in states)
        {
            sb.AppendLine(state.ToString());
        }

        return sb.ToString();
    }
}