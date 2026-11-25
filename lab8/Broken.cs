// Навмисні помилки для чотирьох контрольованих дослідів.
static class Broken
{
    public static int Strict(int[] a, int x)
    {
        int lo = 0, hi = a.Length - 1;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (a[mid] == x) return mid;
            if (a[mid] < x) lo = mid + 1; else hi = mid - 1;
        }
        return -1;
    }
    public static string Stuck(int[] a, int x)
    {
        int lo = 0, hi = a.Length - 1;
        for (int step = 1; lo <= hi && step <= 20; step++)
        {
            int mid = lo + (hi - lo) / 2;
            if (a[mid] == x) return $"індекс {mid}";
            if (a[mid] < x) lo = mid; else hi = mid - 1;
        }
        return $"після 20 кроків lo={lo}, hi={hi}";
    }
    public static int UnsafeMid(int lo, int hi) => unchecked(lo + hi) / 2;
    public static int Unsorted(int[] data, int x) =>
        Search.BinarySearch(data, x, (a, b) => a.CompareTo(b), out _);
}
