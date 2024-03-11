using System.Collections.Generic;

public static class LinqExtensions
{
    /// <summary>
    /// Similar to the JavaScript Array.prototype.entries method.
    /// </summary>
    public static IEnumerable<(int index, T item)> Entries<T>(this IEnumerable<T> source)
    {
        int index = 0;
        foreach (var item in source)
        {
            yield return (index, item);
            index++;
        }
    }

    public static int IndexOf<T>(this IEnumerable<T> source, System.Func<T, bool> predicate)
    {
        int index = 0;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                return index;
            }
            index++;
        }
        return -1; // Not found
    }

    public static IEnumerable<(T a, T b)> Pairwise<T>(this IEnumerable<T> source)
    {
        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            yield break;

        var previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (previous, enumerator.Current);
            previous = enumerator.Current;
        }
    }
}