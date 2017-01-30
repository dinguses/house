using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class TheMissingLinq
{
    public static IEnumerable<KeyValuePair<int, T>> WithIndex<T>(this IEnumerable<T> self)
    {
        int i = 0;
        return self.Select(x => new KeyValuePair<int, T>(i++, x));
    }
}