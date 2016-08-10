using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// Shell-like lexing convenience methods.
/// </summary>
public static class ShLexer
{
    static bool inquote;
    static List<string> tokens = new List<string>();
    static StringBuilder sb = new StringBuilder();

    private static void Reset()
    {
        inquote = false;
        tokens.Clear();
        sb.Length = 0;
    }

    public static string Rejoin(this IEnumerable<string> txt, string separator = " ")
    {
        Reset();
        sb.Append(txt.Take(1).First());
        foreach (var item in txt.Skip(1))
        {
            sb.Append(separator);
            sb.Append(item);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Lex a string, handling whitespace and quotes properly.
    /// 
    /// Unbalanced quotes will fail out by returning null.
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static List<string> Shlex(this string self)
    {
        Reset();
        foreach (var chr in self)
        {
            if (inquote)
            {
                switch (chr)
                {
                    case '"':
                        inquote = false;
                        break;
                    default:
                        sb.Append(chr);
                        break;
                }
            }
            else
            {
                switch (chr)
                {
                    case '"':
                        inquote = true;
                        break;
                    case ' ':
                        if (sb.Length == 0) break;
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                        break;
                    default:
                        sb.Append(chr);
                        break;
                }
            }
        }

        if (inquote) return null;

        if (sb.Length != 0) tokens.Add(sb.ToString());

        return tokens;
    }
}