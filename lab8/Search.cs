static class Search
{
    public static int LinearSearch<T>(T[] a, T x, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        for (int i = 0; i < a.Length; i++)
        {
            comparisons++;
            if (cmp(a[i], x) == 0) return i;
        }
        return -1;
    }
    public static int BinarySearch<T>(T[] a, T x, Func<T, T, int> cmp, out int comparisons)
    {
        comparisons = 0;
        return Finish(a, x, cmp, 0, a.Length - 1, ref comparisons);
    }
    static int Finish<T>(T[] a, T x, Func<T, T, int> cmp, int lo, int hi, ref int comparisons)
    {
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            int order = cmp(a[mid], x);
            comparisons++;
            if (order == 0) return mid;
            if (order < 0) lo = mid + 1;
            else hi = mid - 1;
        }
        return -1;
    }
    public static int Bound<T>(T[] a, Func<T, bool> condition, out int comparisons)
    {
        int lo = 0, hi = a.Length;
        comparisons = 0;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            comparisons++;
            if (condition(a[mid])) hi = mid;
            else lo = mid + 1;
        }
        return lo;
    }
    public static int Interpolation<T>(T[] a, T x, Func<T, T, int> cmp, Func<T, double> key, out int comparisons)
    {
        comparisons = 0;
        if (a.Length == 0) return -1;
        double span = key(a[^1]) - key(a[0]);
        double fraction = span == 0 ? 0 : Math.Clamp((key(x) - key(a[0])) / span, 0, 1);
        int probe = (int)(fraction * (a.Length - 1));
        int order = cmp(a[probe], x);
        comparisons++;
        if (order == 0) return probe;
        return order < 0 ? Finish(a, x, cmp, probe + 1, a.Length - 1, ref comparisons)
                       : Finish(a, x, cmp, 0, probe - 1, ref comparisons);
    }
}
