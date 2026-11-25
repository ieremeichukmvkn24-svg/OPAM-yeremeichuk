static class Checks
{
    public static bool IsSorted<T>(T[] data, Func<T, T, int> compare)
    {
        for (int i = 1; i < data.Length; i++)
            if (compare(data[i - 1], data[i]) > 0) return false;
        return true;
    }
    public static bool IsPermutationOf<T>(T[] before, T[] after)
    {
        if (before.Length != after.Length) return false;
        var counts = new Dictionary<T, int>();
        foreach (T x in before) counts[x] = counts.GetValueOrDefault(x) + 1;
        foreach (T x in after)
        {
            if (counts.GetValueOrDefault(x) == 0) return false;
            counts[x]--;
        }
        return true;
    }
    public static void Verify<T>(T[] before, T[] after, Func<T, T, int> compare)
    {
        if (!IsSorted(after, compare) || !IsPermutationOf(before, after))
            throw new Exception("Помилка порядку або складу масиву");
    }
}
