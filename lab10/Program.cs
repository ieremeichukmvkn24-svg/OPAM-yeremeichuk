using System;
using System.IO;
using System.Globalization;
using System.Text;
using static System.Console;
using static TextAlgorithms;
using static Intervals;

class Program
{
    private const int N = 9;
    private const int TextLength = 50000;

    static double Time(Action run) => Measurements.Best(run);
    static void Main()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "prose.txt");
        string sourceText = File.ReadAllText(path);
        if (sourceText.Length < TextLength)
        {
            throw new InvalidDataException("Файл prose.txt має містити щонайменше 50 000 символів");
        }
        string text = sourceText[..TextLength];
        var words = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        // ЗАВДАННЯ 1: Особливості роботи з рядками.
        WriteLine("## Три пастки\n\n| Додавань | +=, мс | StringBuilder, мс |\n|---:|---:|---:|");
        foreach (int n in new[] { 10000, 100000 })
        {
            double slow = Time(() => { string s = ""; for (int i = 0; i < n; i++) s += 'М'; GC.KeepAlive(s); });
            double fast = Time(() => { var s = new StringBuilder(n); for (int i = 0; i < n; i++) s.Append('М'); GC.KeepAlive(s.ToString()); });
            WriteLine($"| {n} | {slow:F3} | {fast:F3} |");
        }
        WriteLine("\nКожне += копіює вже накопичений префікс. StringBuilder записує символи в буфер.");
        int matches = 0;
        double lower = Time(() => { matches = 0; for (int i = 0; i < 100000; i++) if (words[i % words.Length].ToLower() == words[(i + 1) % words.Length].ToLower()) matches++; });
        double ignore = Time(() => { matches = 0; for (int i = 0; i < 100000; i++) if (string.Equals(words[i % words.Length], words[(i + 1) % words.Length], StringComparison.OrdinalIgnoreCase)) matches++; });
        WriteLine($"\n100 000 пар: ToLower — {lower:F3} мс, OrdinalIgnoreCase — {ignore:F3} мс. Другий спосіб порівнює без створення двох копій слів.");
        string[] sample = "гава ґава ганок ґанок гніт ґніт гра ґрати гудзик ґудзик гора ґазда дім дуб єнот їжак кіт хата яма сад".Split(' ');
        var ordinal = (string[])sample.Clone(); var ukrainian = (string[])sample.Clone();
        double ord = Time(() => { sample.CopyTo(ordinal, 0); Array.Sort(ordinal, StringComparer.Ordinal); });
        double cult = Time(() => { sample.CopyTo(ukrainian, 0); Array.Sort(ukrainian, StringComparer.Create(new CultureInfo("uk-UA"), false)); });
        WriteLine($"\nOrdinal ({ord:F4} мс) — {string.Join(", ", ordinal)}.\n\nuk-UA ({cult:F4} мс) — {string.Join(", ", ukrainian)}. Український порядок ставить ґ після г. В Ordinal позицію визначає код UTF-16.");
        // ЗАВДАННЯ 2: Наївний пошук та ковзне вікно.
        WriteLine("\n## Найдовший підрядок без повторів\n\n| Символів | Наївно, мс | Вікно, мс | Виграш | Довжина |\n|---:|---:|---:|---:|---:|");
        double finalSlow = 0, finalFast = 0;
        foreach (int n in new[] { 1000, 10000, TextLength })
        {
            string slice = text[..n]; Agree(slice);
            finalSlow = Time(() => Naive(slice)); finalFast = Time(() => Window(slice));
            WriteLine($"| {n} | {finalSlow:F4} | {finalFast:F4} | {finalSlow / finalFast:F2} | {Window(slice).Length} |");
        }
        // ЗАВДАННЯ 3: Пошук підрядка.
        WriteLine("\n## Пошук підрядка\n\n| Вхід | Шаблон | Індекс | Порівнянь | Власний, мс | IndexOf, мс |\n|---|---|---:|---:|---:|---:|");
        foreach (var (source, pattern, label) in new[] { (text, "Канівщина", "проза"), (text, "електросамокат", "проза"), (new string('М', 10000), new string('М', 1069) + "Н", "повтори") })
        {
            int pos = TextAlgorithms.IndexOf(source, pattern, out int c);
            if (pos != source.IndexOf(pattern, StringComparison.Ordinal)) throw new Exception();
            WriteLine($"| {label} | {(pattern.Length > 30 ? "1069 × М + Н" : pattern)} | {pos} | {c} | {Time(() => TextAlgorithms.IndexOf(source, pattern, out _)):F4} | {Time(() => source.IndexOf(pattern, StringComparison.Ordinal)):F4} |");
        }
        WriteLine("\nНа повторах кожен початок збігається аж до останньої літери шаблону. Бібліотечний пошук може відкидати кандидатів блоками. Його внутрішні порівняння тут не підраховувалися.");
        // ЗАВДАННЯ 4: Вибір неперетинних інтервалів.
        Random random = new Random(N);
        int wins = 0;
        Interval[] Generate(int n)
        {
            var data = new Interval[n];
            for (int i = 0; i < n; i++) { int start = random.Next(44); data[i] = new(start, random.Next(start + 1, 45)); }
            return data;
        }
        for (int t = 0; t < 100; t++) { var data = Generate(random.Next(1, 16)); if (Greedy(data, Finish) == Exact(data, false, out _)) wins++; }
        if (wins != 100) throw new Exception("Жадібний вибір програв");
        WriteLine($"\n## Заявки [початок, кінець)\n\nОптимум у {wins}/100 тестах. Найгірше відставання 0%. Раніше завершення залишає щонайменше стільки часу для решти заявок, як будь-який інший перший вибір.");
        Interval[] early = [new(0, 10), new(1, 3), new(3, 5)];
        Interval[] shortFirst = [new(0, 4), new(3, 5), new(4, 8)];
        WriteLine($"\nРанній початок: [0,10), [1,3), [3,5) → {Greedy(early, (x, y) => x.Start.CompareTo(y.Start))} замість {Exact(early, false, out _)}.");
        WriteLine($"\nНайкоротша заявка: [0,4), [3,5), [4,8) → {Greedy(shortFirst, (x, y) => (x.End - x.Start).CompareTo(y.End - y.Start))} замість {Exact(shortFirst, false, out _)}.");
        WriteLine("\n| Заявок | Перебір, мс | Відсікання, мс | Вузлів без / з відсіканням |\n|---:|---:|---:|---:|");
        foreach (int n in new[] { 15, 20 })
        {
            var data = Generate(n);
            if (Exact(data, false, out long full) != Exact(data, true, out long cut)) throw new Exception();
            WriteLine($"| {n} | {Time(() => Exact(data, false, out _)):F4} | {Time(() => Exact(data, true, out _)):F4} | {full} / {cut} |");
        }
        // ЗАВДАННЯ 5: Перевірка крайових випадків.
        WriteLine("\n## Крайові випадки\n\n| Запит | Очікувано | Отримано |\n|---|---|---|");
        foreach (var (s, expected) in new[] { ("", ""), ("я", "я"), ("ммм", "м"), ("  аб  в ", " аб") })
        {
            Agree(s); string actual = Window(s);
            if (actual != expected) throw new Exception("Межа вікна");
            WriteLine($"| вікно «{s}» | «{expected}» | «{actual}» |");
        }
        foreach (var (s, p, expected) in new[] { ("", "а", -1), ("а", "аб", -1), ("аб", "", 0) })
        {
            int actual = TextAlgorithms.IndexOf(s, p, out _); if (actual != expected) throw new Exception();
            WriteLine($"| пошук «{p}» у «{s}» | {expected} | {actual} |");
        }
        foreach (Interval[] data in new Interval[][] { [], [new(1, 2)] })
        {
            int actual = Greedy(data, Finish); if (actual != data.Length || Exact(data, false, out _) != actual) throw new Exception();
            WriteLine($"| заявок {data.Length} | {data.Length} | {actual} |");
        }
        WriteLine($"\n## Вибір алгоритму\n\n1. Вхід — уривок прози, найбільший замір на 50 000 символів.\n2. На кожен із трьох розмірів потрібен один найдовший підрядок. За рівної довжини повертається перший.\n3. Текст між запитами сталий, для нового уривка вікно запускається з початку.\n4. На повному уривку важливий час: {finalSlow:F4} проти {finalFast:F4} мс. Наївний метод повторно перевіряє пари, швидкий зберігає множину символів поточного вікна.\n5. HashSet<char> дає додавання й вилучення символу в середньому за сталий час.\n6. На 50 000 символів обрано ковзне вікно, виграш {finalSlow / finalFast:F2} раза. Короткі серії без повторів у прозі обмежують витрати наївного методу.");
    }
}
