using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Extensions;
public static class IEnumerableExtensions
{
    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        return source.Select(item =>
        {
            action(item);
            return item;
        });
    }
}
