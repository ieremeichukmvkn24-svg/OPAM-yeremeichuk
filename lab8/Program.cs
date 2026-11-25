using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using static Search;

class Program
{
    private const int N = 9;
    private const int K = 17;
    private const int KeyRange = 20 * K + 1;

    static readonly Func<int, int, int> Compare = (a, b) => a.CompareTo(b);
    static int[] Data(int n)
    {
        var a = new int[n];
        var r = new Random(N);
        for (int i = 0; i < n; i++) a[i] = r.Next(KeyRange);
        return a;
    }
    static void Main()
    {
        int[] raw = Data(100000);
        int[] a = (int[])raw.Clone();
        Array.Sort(a);
        WriteLine("## Пошук на 100 000 чисел\n\n| Запит | Лінійний: індекс / порівняння | Бінарний | Інтерполяція |\n|---|---|---|---|");
        foreach (int x in new[] { 10 * K, a[0], KeyRange })
        {
            int l = LinearSearch(a, x, Compare, out int lc);
            int b = BinarySearch(a, x, Compare, out int bc);
            int p = Interpolation(a, x, Compare, v => v, out int pc);
            if ((l < 0) != (b < 0) || (b < 0) != (p < 0) ||
                (l >= 0 && a[l] != x) || (b >= 0 && a[b] != x) ||
                (p >= 0 && a[p] != x))
            {
                throw new Exception("Алгоритми пошуку повернули різні результати");
            }
            WriteLine($"| {x} | {l} / {lc} | {b} / {bc} | {p} / {pc} |");
        }
        WriteLine("\n## Чотири поломки\n");
        WriteLine($"1. [1], шукаємо 1. Умова lo < hi пропускає єдину позицію, результат {Broken.Strict([1], 1)}.");
        WriteLine($"2. [1, 2], шукаємо 2. Присвоєння lo = mid залишає межі на місці: {Broken.Stuck([1, 2], 2)}.");
        WriteLine($"3. Для довжини 1 073 741 825 останній індекс 1 073 741 824. Коли обидві межі дійдуть до нього, (lo + hi) / 2 дає {Broken.UnsafeMid(1073741824, 1073741824)}. Для 100 000 сума індексів щонайбільше 199 998.");
        int missed = 0;
        var r = new Random(N);
        for (int i = 0; i < 100; i++) if (Broken.Unsorted(raw, raw[r.Next(raw.Length)]) < 0) missed++;
        WriteLine($"4. Без сортування пропущено {missed} зі 100 наявних запитів.");
        int lower = Bound(a, v => v >= 10 * K, out int lowCost);
        int upper = Bound(a, v => v > 10 * K, out int highCost);
        int last = -1, count = 0;
        for (int i = 0; i < a.Length; i++) if (a[i] == 10 * K) { last = i; count++; }
        if (last != upper - 1 || count != upper - lower) throw new Exception();
        WriteLine($"\n## Межі\n\nБлок [ {lower}, {upper} ). Останній індекс {last} за {highCost} порівнянь; кількість {count} за {lowCost + highCost}. Контроль повним обходом — {a.Length} порівнянь для обох відповідей. На кожній ітерації лишається половина ділянки незалежно від кількості повторів.");
        int invalid = Bound(a, v => v % 7 == 0, out _);
        WriteLine($"Умова кратності 7 чергує істину й хибу. Метод дав {invalid}, перша правильна позиція 0 (значення {a[0]}).");
        WriteLine("\n## Середні порівняння для 100 запитів\n\n| n | Лінійний | Бінарний | Відношення | n/2 | log₂ n |\n|---:|---:|---:|---:|---:|---:|");
        foreach (int n in new[] { 1000, 10000, 100000, 1000000 })
        {
            var data = Data(n); Array.Sort(data); r = new Random(N);
            long lc = 0, bc = 0;
            for (int i = 0; i < 100; i++) { int x = r.Next(KeyRange); LinearSearch(data, x, Compare, out int l); BinarySearch(data, x, Compare, out int b); lc += l; bc += b; }
            WriteLine($"| {n} | {lc / 100.0:F2} | {bc / 100.0:F2} | {(double)lc / bc:F2} | {n / 2} | {Math.Log2(n):F2} |");
        }
        WriteLine("\n## Окупність сортування\n\n| q | Перебір, мс | Сортування + пошук, мс | Швидший |\n|---:|---:|---:|---|");
        r = new Random(N);
        var queries = new int[51200];
        for (int i = 0; i < queries.Length; i++) queries[i] = r.Next(KeyRange);
        int first = 0;
        foreach (int q in new[] { 1, 5, 10, 20, 30, 50, 100, 200, 400, 800, 1600, 3200, 6400, 12800, 25600, 51200 })
        {
            double linear = Measurements.Best(() => { foreach (int x in queries.AsSpan(0, q)) LinearSearch(raw, x, Compare, out _); });
            double binary = Measurements.Best(() => { var sorted = (int[])raw.Clone(); Array.Sort(sorted); foreach (int x in queries.AsSpan(0, q)) BinarySearch(sorted, x, Compare, out _); });
            WriteLine($"| {q} | {linear:F4} | {binary:F4} | {(linear < binary ? "перебір" : "бінарний із сортуванням")} |");
            if (binary < linear && first == 0) first = q;
            if (q >= 100 && first > 0) break;
        }
        WriteLine($"\nПерша виміряна перемога сортування: {(first == 0 ? "поза перевіреним діапазоном" : first.ToString())}. Оцінка 34 припускає довгий перебір. Тут лише 341 можливий ключ, тому на випадковому невпорядкованому вході копія ключа зазвичай трапляється значно раніше за середину.");
        WriteLine("\n## Крайові випадки\n\n| Масив, запит | Очікувані індекс / межі | Отримано |\n|---|---|---|");
        foreach (var (data, x, expected, lo, hi) in new (int[], int, int, int, int)[] { ([], 1, -1, 0, 0), ([1], 1, 0, 0, 1), ([1, 2], 2, 1, 1, 2), ([2, 3], 1, -1, 0, 0), ([2, 3], 4, -1, 2, 2), ([2, 2, 2], 2, 1, 0, 3) })
        {
            int idx = BinarySearch(data, x, Compare, out _), left = Bound(data, v => v >= x, out _), right = Bound(data, v => v > x, out _);
            if (idx != expected || left != lo || right != hi) throw new Exception();
            WriteLine($"| [{string.Join(",", data)}], {x} | {expected} / {lo}, {hi} | {idx} / {left}, {right} |");
        }
    }
}

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

static class Measurements
{
    public static double Best(Action run)
    {
        run();
        double best = double.MaxValue;
        for (int trial = 0; trial < 3; trial++)
        {
            var timer = Stopwatch.StartNew();
            run();
            timer.Stop();
            best = Math.Min(best, timer.Elapsed.TotalMilliseconds);
        }
        return best;
    }
}
