using System;
using System.Collections.Generic;

// --- Бібліотека алгоритмів в одному файлі (щоб уникнути помилок контексту) ---
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

class Student
{
    public string Name;
    public string Group;
    public int Score;

    public Student(string name, string group, int score)
    {
        Name = name;
        Group = group;
        Score = score;
    }
}

class Program
{
    static void Main()
    {
        int N = 9;
        int K = 17;
        int a = N % 3; // 0
        int b = N % 4; // 1
        int c = N % 5; // 4

        Console.WriteLine($"=== ВАРІАНТ: N = {N}, K = {K}, a = {a}, b = {b}, c = {c} ===");

        // --- ПІДГОТОВКА ТРИХ НАБОРІВ ДАНИХ ---
        List<int> intList = GenerateRandomList(20, -K, K, N);
        List<string> strList = new List<string> {
            "алгоритм", "код", "програма", "комп'ютер", "C#", "дані", 
            "тест", "розробка", "функція-делегат", "вираз", "об'єкт", "система"
        };
        List<Student> studentList = new List<Student>
        {
            new Student("Коваленко", "КН-3/1", 88),
            new Student("Мельник",    "КН-3/2", 64),
            new Student("Іщенко",     "КН-3/1", 93),
            new Student("Бондар",     "КН-3/1", 71),
            new Student("Савченко",   "КН-3/2", 55),
            new Student("Ткачук",     "КН-3/2", 90),
            new Student("Кравець",    "КН-3/1", 76),
            new Student("Іванів",     "КН-3/1", 48),
            new Student("Мороз",      "КН-3/2", 82),
            new Student("Шевчук",     "КН-3/1", 67),
            new Student("Бойко",      "КН-3/2", 95),
            new Student("Кушнір",     "КН-3/2", 73)
        };

        // ---------------------------------------------------------
        // ЗАВДАННЯ 1: Відбір (Filter) з трьома критеріями (для а = 0)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 1: Фільтрація ---");
        var filteredInts = Algorithms.Filter(intList, x => x > K);
        var filteredStrs = Algorithms.Filter(strList, s => s.Length > 5);
        var filteredStudents = Algorithms.Filter(studentList, st => st.Score >= 75);

        Console.WriteLine($"Числа (> {K}): {filteredInts.Count} шт. -> [ {string.Join(", ", filteredInts)} ]");
        Console.WriteLine($"Рядки (> 5 символів): {filteredStrs.Count} шт. -> [ {string.Join(", ", filteredStrs)} ]");
        Console.WriteLine($"Студенти (бал >= 75): {filteredStudents.Count} шт.");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 2: Перетворення (Map) з трьома правилами (для b = 1)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 2: Перетворення (Map) ---");
        int modDiv = K % 7 + 2; // 17 % 7 + 2 = 5
        var mappedInts = Algorithms.Map(intList, x => x % modDiv);
        var mappedStrs = Algorithms.Map(strList, s => s.ToUpper());
        var mappedStudents = Algorithms.Map(studentList, st => $"{st.Name} ({st.Score})");

        Console.WriteLine($"Числа (остача від діл. на {modDiv}): [ {string.Join(", ", mappedInts)} ]");
        Console.WriteLine($"Рядки (Upper): [ {string.Join(", ", mappedStrs)} ]");
        Console.WriteLine($"Студенти (формат): [ {string.Join("; ", mappedStudents)} ]");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 3: Агрегація (Aggregate)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 3: Агрегація ---");
        int sum = Algorithms.Aggregate(intList, 0, (acc, x) => acc + x);
        int product = Algorithms.Aggregate(intList, 1, (acc, x) => acc * x);
        int max = intList.Count > 0 ? Algorithms.Aggregate(intList, intList[0], (acc, x) => x > acc ? x : acc) : 0;
        
        var avgTuple = Algorithms.Aggregate(intList, (sum: 0, count: 0), (acc, x) => (acc.sum + x, acc.count + 1));
        double average = avgTuple.count > 0 ? (double)avgTuple.sum / avgTuple.count : 0;

        Console.WriteLine($"Сума чисел: {sum}");
        Console.WriteLine($"Добуток чисел: {product}");
        Console.WriteLine($"Максимум чисел: {max}");
        Console.WriteLine($"Середнє арифметичне: {average:F2} (Сума: {avgTuple.sum}, Кількість: {avgTuple.count})");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 4: Групування (GroupBy)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 4: Групування ---");
        var groupByName = Algorithms.GroupBy(studentList, st => st.Group);
        Console.WriteLine("Групування студентів за навчальною групою:");
        foreach (var kvp in groupByName)
            Console.WriteLine($"  Група {kvp.Key}: {kvp.Value.Count} студентів");

        var groupByLetter = Algorithms.GroupBy(studentList, st => st.Name[0]);
        Console.WriteLine("Групування студентів за першою літерою прізвища:");
        foreach (var kvp in groupByLetter)
            Console.WriteLine($"  Літера '{kvp.Key}': {kvp.Value.Count} студентів");

        var groupByScoreRange = Algorithms.GroupBy(studentList, st => {
            if (st.Score <= 59) return "0-59";
            if (st.Score <= 74) return "60-74";
            if (st.Score <= 89) return "75-89";
            return "90-100";
        });
        Console.WriteLine("Групування студентів за діапазоном балу:");
        foreach (var kvp in groupByScoreRange)
            Console.WriteLine($"  Діапазон {kvp.Key}: {kvp.Value.Count} студентів");

        var groupByLength = Algorithms.GroupBy(strList, s => s.Length);
        Console.WriteLine("Групування рядків за довжиною:");
        foreach (var kvp in groupByLength)
            Console.WriteLine($"  Довжина {kvp.Key}: {kvp.Value.Count} слів");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 5: Захоплення змінних (Замикання)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 5: Замикання (Несподіванки) ---");
        
        int threshold = K;
        Func<int, bool> checkThresh = x => x > threshold;
        Console.WriteLine($"Перевірка 20 > threshold ({threshold}): {checkThresh(20)}");
        threshold = 1000;
        Console.WriteLine($"Перевірка 20 > threshold після зміни на 1000: {checkThresh(20)}");

        List<Func<int>> funcsFor = new List<Func<int>>();
        for (int i = 0; i < 3; i++)
        {
            funcsFor.Add(() => i);
        }
        Console.WriteLine($"Результати виклику лямбд із циклу for: {funcsFor[0]()}, {funcsFor[1]()}, {funcsFor[2]()}");

        int[] arr = { 0, 1, 2 };
        List<Func<int>> funcsForeach = new List<Func<int>>();
        foreach (int item in arr)
        {
            funcsForeach.Add(() => item);
        }
        Console.WriteLine($"Результати виклику лямбд із foreach: {funcsForeach[0]()}, {funcsForeach[1]()}, {funcsForeach[2]()}");
    }

    static List<int> GenerateRandomList(int count, int min, int max, int seed)
    {
        Random rnd = new Random(seed);
        List<int> list = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            list.Add(rnd.Next(min, max + 1));
        }
        return list;
    }
}