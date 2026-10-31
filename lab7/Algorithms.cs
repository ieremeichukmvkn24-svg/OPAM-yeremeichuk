using System;
using System.Collections.Generic;

public static class Algorithms
{
    // Завдання 1: Узагальнений фільтр
    public static List<T> Filter<T>(List<T> src, Func<T, bool> keep)
    {
        List<T> result = new List<T>();
        if (src == null) return result;

        foreach (var item in src)
        {
            if (keep(item))
            {
                result.Add(item);
            }
        }
        return result;
    }

    // Завдання 2: Узагальнене перетворення (Map) із двома типовими параметрами
    public static List<TOut> Map<TIn, TOut>(List<TIn> src, Func<TIn, TOut> convert)
    {
        List<TOut> result = new List<TOut>();
        if (src == null) return result;

        foreach (var item in src)
        {
            result.Add(convert(item));
        }
        return result;
    }

    // Завдання 3: Узагальнена агрегація
    public static TAcc Aggregate<T, TAcc>(List<T> src, TAcc start, Func<TAcc, T, TAcc> step)
    {
        TAcc acc = start;
        if (src == null) return acc;

        foreach (var item in src)
        {
            acc = step(acc, item);
        }
        return acc;
    }

    // Завдання 4: Узагальнене групування
    public static Dictionary<TKey, List<T>> GroupBy<T, TKey>(List<T> src, Func<T, TKey> keyOf)
    {
        Dictionary<TKey, List<T>> groups = new Dictionary<TKey, List<T>>();
        if (src == null) return groups;

        foreach (var item in src)
        {
            TKey key = keyOf(item);
            if (!groups.ContainsKey(key))
            {
                groups[key] = new List<T>();
            }
            groups[key].Add(item);
        }

        return groups;
    }
}