using System;
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
        // ЗАВДАННЯ 1: Порівняння трьох алгоритмів пошуку.
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
        // ЗАВДАННЯ 2: Навмисні помилки бінарного пошуку.
        WriteLine("\n## Чотири поломки\n");
        WriteLine($"1. [1], шукаємо 1. Умова lo < hi пропускає єдину позицію, результат {Broken.Strict([1], 1)}.");
        WriteLine($"2. [1, 2], шукаємо 2. Присвоєння lo = mid залишає межі на місці: {Broken.Stuck([1, 2], 2)}.");
        WriteLine($"3. Для довжини 1 073 741 825 останній індекс 1 073 741 824. Коли обидві межі дійдуть до нього, (lo + hi) / 2 дає {Broken.UnsafeMid(1073741824, 1073741824)}. Для 100 000 сума індексів щонайбільше 199 998.");
        int missed = 0;
        var r = new Random(N);
        for (int i = 0; i < 100; i++) if (Broken.Unsorted(raw, raw[r.Next(raw.Length)]) < 0) missed++;
        WriteLine($"4. Без сортування пропущено {missed} зі 100 наявних запитів.");
        // ЗАВДАННЯ 3: Межі блока однакових значень.
        int lower = Bound(a, v => v >= 10 * K, out int lowCost);
        int upper = Bound(a, v => v > 10 * K, out int highCost);
        int last = -1, count = 0;
        for (int i = 0; i < a.Length; i++) if (a[i] == 10 * K) { last = i; count++; }
        if (last != upper - 1 || count != upper - lower) throw new Exception();
        WriteLine($"\n## Межі\n\nБлок [ {lower}, {upper} ). Останній індекс {last} за {highCost} порівнянь; кількість {count} за {lowCost + highCost}. Контроль повним обходом — {a.Length} порівнянь для обох відповідей. На кожній ітерації лишається половина ділянки незалежно від кількості повторів.");
        int invalid = Bound(a, v => v % 7 == 0, out _);
        WriteLine($"Умова кратності 7 чергує істину й хибу. Метод дав {invalid}, перша правильна позиція 0 (значення {a[0]}).");
        // ЗАВДАННЯ 4: Підрахунок порівнянь.
        WriteLine("\n## Середні порівняння для 100 запитів\n\n| n | Лінійний | Бінарний | Відношення | n/2 | log₂ n |\n|---:|---:|---:|---:|---:|---:|");
        foreach (int n in new[] { 1000, 10000, 100000, 1000000 })
        {
            var data = Data(n); Array.Sort(data); r = new Random(N);
            long lc = 0, bc = 0;
            for (int i = 0; i < 100; i++) { int x = r.Next(KeyRange); LinearSearch(data, x, Compare, out int l); BinarySearch(data, x, Compare, out int b); lc += l; bc += b; }
            WriteLine($"| {n} | {lc / 100.0:F2} | {bc / 100.0:F2} | {(double)lc / bc:F2} | {n / 2} | {Math.Log2(n):F2} |");
        }
        // ЗАВДАННЯ 5: Вимірювання часу.
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
        // Перевірка крайових випадків.
        WriteLine("\n## Крайові випадки\n\n| Масив, запит | Очікувані індекс / межі | Отримано |\n|---|---|---|");
        foreach (var (data, x, expected, lo, hi) in new (int[], int, int, int, int)[] { ([], 1, -1, 0, 0), ([1], 1, 0, 0, 1), ([1, 2], 2, 1, 1, 2), ([2, 3], 1, -1, 0, 0), ([2, 3], 4, -1, 2, 2), ([2, 2, 2], 2, 1, 0, 3) })
        {
            int idx = BinarySearch(data, x, Compare, out _), left = Bound(data, v => v >= x, out _), right = Bound(data, v => v > x, out _);
            if (idx != expected || left != lo || right != hi) throw new Exception();
            WriteLine($"| [{string.Join(",", data)}], {x} | {expected} / {lo}, {hi} | {idx} / {left}, {right} |");
        }
    }
}

