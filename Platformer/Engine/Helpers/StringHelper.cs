using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Platformer.Engine.Helpers;

public static class StringHelper
{
    public static Dictionary<string, Collection<string>> GroupByCommonPrefix(char separator, string[] items)
    {
        if (items.Length is 0) return [];
        if (items.Length is 1) return new Dictionary<string, Collection<string>> { [items[0]] = [items[0]] };

        var sorted = items.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
        var result = new Dictionary<string, Collection<string>>();

        GroupRecursive(separator, sorted, result);

        return result;
    }

    private static void GroupRecursive(char separator, string[] items, Dictionary<string, Collection<string>> result)
    {
        while (true)
        {
            if (items.Length is 0) return;

            if (items.Length is 1)
            {
                result[items[0]] = [items[0]];
                return;
            }

            var lcps = new int[items.Length - 1];
            for (var i = 0; i < items.Length - 1; i++) lcps[i] = LongestCommonPrefix(items[i], items[i + 1]);

            if (lcps.All(l => l == lcps[0]))
            {
                if (lcps[0] is 0)
                {
                    foreach (var item in items) result[item] = [item];
                }
                else
                {
                    var key = TrimToLastSeparator(separator, items[0][..lcps[0]]);
                    result[key] = [.. items];
                }

                return;
            }

            var minLcp = lcps.Min();
            var splitIndex = Array.IndexOf(lcps, minLcp);

            GroupRecursive(separator, items[..(splitIndex + 1)], result);
            items = items[(splitIndex + 1)..];
        }
    }

    private static int LongestCommonPrefix(string a, string b)
    {
        var length = Math.Min(a.Length, b.Length);
        var i = 0;
        while (i < length && char.ToLower(a[i]) == char.ToLower(b[i]))
            i++;
        return i;
    }

    private static string TrimToLastSeparator(char separator, string prefix)
    {
        var lastSep = prefix.LastIndexOf(separator);
        return lastSep >= 0 ? prefix[..lastSep] : prefix;
    }
}