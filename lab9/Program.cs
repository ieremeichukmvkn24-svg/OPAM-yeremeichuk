using static System.Console;
using static Sorts;
using static Checks;

class Program
{
    private const int N = 9;
    private const int K = 17;
    private const int KeyRange = 20 * K + 1;

    record Student(string Name, int Group, int Score);
    static readonly Func<int, int, int> Compare = (x, y) => x.CompareTo(y);
    static readonly Func<Student, Student, int> Keys = (x, y) => x.Group != y.Group ? x.Group.CompareTo(y.Group) : x.Score.CompareTo(y.Score);
    static Student[] Students(int[] values)
    {
        var result = new Student[values.Length];
        for (int i = 0; i < result.Length; i++) result[i] = new Student($"Студент{i:D7}", values[i] % 7, values[i] / 7);
        return result;
    }
    static int[] Data(int n)
    {
        var random = new Random(N);
        var data = new int[n];
        for (int i = 0; i < n; i++) data[i] = random.Next(KeyRange);
        return data;
    }
    static void TwoPass(Student[] data, out long comparisons, out long moves)
    {
        Merge(data, (x, y) => x.Score.CompareTo(y.Score), out long c, out long m);
        Merge(data, (x, y) => x.Group.CompareTo(y.Group), out comparisons, out moves);
        comparisons += c;
        moves += m;
    }
    static void Main()
    {
        WriteLine("## Трасування");
        var trace = Data(8);
        WriteLine("Початок → " + string.Join(", ", trace));
        Selection(trace, Compare, out _, out _, row => WriteLine("\n→ " + string.Join(", ", row)));
        WriteLine("\n## Порівняння / записи (включно з буфером)\n");
        WriteLine("| n | Вхід | Вибір | Злиття | Складений ключ | Два проходи |\n|---:|---|---:|---:|---:|---:|");
        foreach (int n in new[] { 100, 1000, 10000 })
            foreach (string kind in new[] { "випадковий", "зростає", "спадає", "майже" })
            {
                var input = Data(n);
                if (kind != "випадковий") Array.Sort(input);
                if (kind == "спадає") Array.Reverse(input);
                if (kind == "майже")
                {
                    var r = new Random(N);
                    for (int i = 0; i < K; i++) { int x = r.Next(n), y = r.Next(n); (input[x], input[y]) = (input[y], input[x]); }
                }
                var a = (int[])input.Clone();
                Selection(a, Compare, out long c, out long m);
                Verify(input, a, Compare);
                a = (int[])input.Clone();
                Merge(a, Compare, out long d, out long k);
                Verify(input, a, Compare);
                var students = Students(input);
                var one = (Student[])students.Clone();
                var two = (Student[])students.Clone();
                Merge(one, Keys, out long e, out long f);
                TwoPass(two, out long g, out long h);
                Verify(students, one, Keys);
                Verify(students, two, Keys);
                if (!one.AsSpan().SequenceEqual(two)) throw new Exception("Розбіжність ключів");
                WriteLine($"| {n} | {kind} | {c}/{m} | {d}/{k} | {e}/{f} | {g}/{h} |");
                if (n == 10000 && kind == "випадковий") Ratio = (double)c / d;
            }
        WriteLine("\n## Час у мс (мінімум із трьох після прогріву)\n");
        WriteLine("| n | Вибір | Злиття | Два ключі (один / два проходи) | Array.Sort |\n|---:|---:|---:|---:|---:|");
        foreach (int n in new[] { 1000, 10000, 100000, 1000000 })
        {
            var a = Data(n);
            var s = Students(a);
            string simple = n == 1000000 ? "—" : Measurements.Best(() => Selection((int[])a.Clone(), Compare, out _, out _)).ToString("F3");
            WriteLine($"| {n} | {simple} | {Measurements.Best(() => Merge((int[])a.Clone(), Compare, out _, out _)):F3} | {Measurements.Best(() => Merge((Student[])s.Clone(), Keys, out _, out _)):F3} / {Measurements.Best(() => TwoPass((Student[])s.Clone(), out _, out _)):F3} | {Measurements.Best(() => Array.Sort((int[])a.Clone())):F3} |");
        }
        for (int n = 5000; ; n += 5000)
        {
            var a = Data(n);
            double ms = Measurements.Best(() => Selection((int[])a.Clone(), Compare, out _, out _));
            if (ms > 1000) { WriteLine($"\nПерший перевірений розмір понад секунду (крок 5000) — {n}, {ms:F1} мс."); break; }
        }
        WriteLine("\n## Стабільність за балом\n\n| Метод | Очікувано | Фактично на вибірці | Перша інверсія прізвищ |\n|---|---|---|---|");
        var sample = Students(Data(120));
        for (int method = 0; method < 4; method++)
        {
            var sorted = (Student[])sample.Clone();
            Func<Student, Student, int> score = (x, y) => x.Score.CompareTo(y.Score);
            if (method == 0) Selection(sorted, score, out _, out _);
            if (method == 1) Merge(sorted, score, out _, out _);
            if (method == 2) Array.Sort(sorted, (x, y) => score(x, y));
            if (method == 3) sorted = sorted.OrderBy(x => x.Score).ToArray();
            Verify(sample, sorted, score);
            string pair = "—";
            for (int i = 1; i < sorted.Length; i++)
                if (sorted[i].Score == sorted[i - 1].Score && string.CompareOrdinal(sorted[i - 1].Name, sorted[i].Name) > 0)
                { pair = sorted[i - 1].Name + " → " + sorted[i].Name; break; }
            WriteLine($"| {new[] { "Вибір", "Злиття", "Array.Sort", "OrderBy" }[method]} | {(method % 2 == 1 ? "стабільний" : "нестабільний")} | {(pair == "—" ? "збережено" : "порушено")} | {pair} |");
        }
        WriteLine("\n## Крайові випадки\n\n| Вхід | Очікувано | Отримано |\n|---|---|---|");
        foreach (int[] a in new int[][] { [], [1], [1, 2], [2, 1], [3, 3, 3], [2, 2, 1, 1] })
        {
            var x = (int[])a.Clone();
            Selection(x, Compare, out _, out _);
            Verify(a, x, Compare);

            x = (int[])a.Clone();
            Merge(x, Compare, out _, out _);
            Verify(a, x, Compare);

            Student[] s = Students(a);
            Student[] t = (Student[])s.Clone();
            TwoPass(t, out _, out _);
            Verify(s, t, Keys);

            t = (Student[])s.Clone();
            Merge(t, Keys, out _, out _);
            Verify(s, t, Keys);
            WriteLine($"| [{string.Join(",", a)}] | порядок і склад збережені | пройдено |");
        }
        WriteLine($"\nНа випадковому вході з 10 000 чисел вибір зробив у {Ratio:F2} раза більше порівнянь. Лекційні 376 спираються на n log₂ n, а тут пораховані фактичні виклики компаратора.");
    }
    static double Ratio;
}
