using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        Console.WriteLine($"=== ВАРІАНТ: N = {N}, K = {K} ===");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 1: Підрахунок частот слів
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 1: Частоти слів ---");
        string text = "Алгоритм описує послідовність дій. Алгоритм можна записати словами, " +
                      "а можна записати кодом. Кодом алгоритм стає тоді, коли його " +
                      "виконує машина, а не людина. Одна задача — багато алгоритмів.";
        
        int threshold = K % 3 + 2; // 17 % 3 + 2 = 4
        Task1_WordFrequency(text, threshold);

        // ---------------------------------------------------------
        // ЗАВДАННЯ 2: Групування студентів за навчальною групою
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 2: Групування студентів ---");
        List<Student> students = new List<Student>
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
        Task2_GroupStudents(students);

        // ---------------------------------------------------------
        // ЗАВДАННЯ 3: Вилучення дублікатів (25 чисел, діапазон [1; 13])
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 3: Вилучення дублікатів ---");
        int maxVal = K % 10 + 6; // 17 % 10 + 6 = 13
        List<int> numbers25 = GenerateRandomList(25, 1, maxVal, N);
        Console.WriteLine("Вихідний список чисел: " + string.Join(", ", numbers25));

        List<int> uniqueNoOrder = Task3_RemoveDuplicatesHashSet(numbers25);
        List<int> uniqueOrdered = Task3_RemoveDuplicatesPreserveOrder(numbers25);

        Console.WriteLine("Без збереження порядку (HashSet): " + string.Join(", ", uniqueNoOrder));
        Console.WriteLine("З збереженням порядку (List + HashSet): " + string.Join(", ", uniqueOrdered));

        // ---------------------------------------------------------
        // ЗАВДАННЯ 4: Порівняння двох наборів (об'єднання без дублікатів)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 4: Об'єднання двох наборів ---");
        int maxSetVal = 20 + K; // 20 + 17 = 37
        List<int> setA = GenerateRandomList(30, 1, maxSetVal, N);
        List<int> setB = GenerateRandomList(30, 1, maxSetVal, N + 100);

        List<int> unionHashSet = Task4_UnionHashSet(setA, setB);
        List<int> unionManual = Task4_UnionManualLists(setA, setB);

        Console.WriteLine($"Розмір набору A: {setA.Count}, Набору B: {setB.Count}, Сума розмірів: {setA.Count + setB.Count}");
        Console.WriteLine($"Розмір об'єднання (HashSet): {unionHashSet.Count}");
        Console.WriteLine($"Розмір об'єднання (Manual): {unionManual.Count}");
        
        bool setsAreEqual = (unionHashSet.Count == unionManual.Count);
        Console.WriteLine($"Результати обох способів збігаються: {setsAreEqual}");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 5: Замір продуктивності
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 5: Вимірювання продуктивності (Бенчмарки) ---");
        RunBenchmarks();
    }

    // --- Допоміжні методи ---

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

    // --- Реалізація Завдання 1 ---
    static void Task1_WordFrequency(string text, int minThreshold)
    {
        // Очищення пунктуації та розбиття на слова без урахування регістру
        char[] separators = { ' ', '.', ',', '—', '!', '?', ':', ';', '-', '\n', '\r', '\t' };
        string[] rawWords = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        Dictionary<string, int> freqDict = new Dictionary<string, int>();

        foreach (var rawWord in rawWords)
        {
            string word = rawWord.ToLower();
            if (freqDict.ContainsKey(word))
            {
                freqDict[word]++;
            }
            else
            {
                freqDict[word] = 1;
            }
        }

        Console.WriteLine($"Слова з частотою не меншою за {minThreshold}:");
        int uniqueCount = 0;

        foreach (var kvp in freqDict)
        {
            uniqueCount++;
            if (kvp.Value >= minThreshold)
            {
                Console.WriteLine($"  - \"{kvp.Key}\": {kvp.Value} раз(и)");
            }
        }
        Console.WriteLine($"Загальна кількість різних елементів (слів): {uniqueCount}");
    }

    // --- Реалізація Завдання 2 ---
    static void Task2_GroupStudents(List<Student> students)
    {
        Dictionary<string, List<Student>> groups = new Dictionary<string, List<Student>>();

        foreach (var student in students)
        {
            string key = student.Group; // Група як ключ
            if (!groups.ContainsKey(key))
            {
                groups[key] = new List<Student>();
            }
            groups[key].Add(student);
        }

        foreach (var kvp in groups)
        {
            string groupName = kvp.Key;
            List<Student> list = kvp.Value;

            double sumScore = 0;
            foreach (var st in list)
            {
                sumScore += st.Score;
            }
            double avgScore = list.Count > 0 ? sumScore / list.Count : 0;

            Console.WriteLine($"Група: {groupName} | Кількість студентів: {list.Count} | Середній бал: {avgScore:F2}");
        }
    }

    // --- Реалізація Завдання 3 ---
    static List<int> Task3_RemoveDuplicatesHashSet(List<int> source)
    {
        HashSet<int> set = new HashSet<int>(source);
        List<int> result = new List<int>(set);
        return result;
    }

    static List<int> Task3_RemoveDuplicatesPreserveOrder(List<int> source)
    {
        HashSet<int> seen = new HashSet<int>();
        List<int> result = new List<int>();

        foreach (var num in source)
        {
            // Add повертає true, якщо елемент успішно додано (його ще не було)
            if (seen.Add(num))
            {
                result.Add(num);
            }
        }
        return result;
    }

    // --- Реалізація Завдання 4 ---
    static List<int> Task4_UnionHashSet(List<int> setA, List<int> setB)
    {
        HashSet<int> union = new HashSet<int>(setA);
        foreach (var item in setB)
        {
            union.Add(item);
        }
        return new List<int>(union);
    }

    static List<int> Task4_UnionManualLists(List<int> setA, List<int> setB)
    {
        List<int> result = new List<int>();

        foreach (var item in setA)
        {
            bool exists = false;
            foreach (var res in result)
            {
                if (res == item) { exists = true; break; }
            }
            if (!exists) result.Add(item);
        }

        foreach (var item in setB)
        {
            bool exists = false;
            foreach (var res in result)
            {
                if (res == item) { exists = true; break; }
            }
            if (!exists) result.Add(item);
        }

        return result;
    }

    // --- Реалізація Завдання 5 (Бенчмарки) ---
    static void RunBenchmarks()
    {
        Stopwatch sw = new Stopwatch();

        // Прогрівальний прогін
        List<int> warmup = new List<int>();
        for (int i = 0; i < 1000; i++) warmup.Add(i);

        // 1. Пошук 10 000 елементів у великому наборі (100 000 елементів)
        List<int> bigList = GenerateRandomList(100000, 1, 500000, 42);
        HashSet<int> bigHashSet = new HashSet<int>(bigList);
        List<int> searchQueries = GenerateRandomList(10000, 1, 500000, 99);

        // Список (Contains)
        sw.Restart();
        long listHits = 0;
        for (int i = 0; i < searchQueries.Count; i++)
        {
            if (bigList.Contains(searchQueries[i])) listHits++;
        }
        sw.Stop();
        long listTime1 = sw.ElapsedMilliseconds;

        // Множина (Contains)
        sw.Restart();
        long setHits = 0;
        for (int i = 0; i < searchQueries.Count; i++)
        {
            if (bigHashSet.Contains(searchQueries[i])) setHits++;
        }
        sw.Stop();
        long setTime1 = sw.ElapsedMilliseconds;

        Console.WriteLine($"1. Пошук у наборі: Список = {listTime1} мс, Множина = {setTime1} мс");

        // 2. Вилучення дублікатів із 20 000 чисел
        List<int> dupList = GenerateRandomList(20000, 1, 5000, 77);

        sw.Restart();
        List<int> resListDup = Task3_RemoveDuplicatesPreserveOrder(dupList);
        sw.Stop();
        long listTime3 = sw.ElapsedMilliseconds;

        sw.Restart();
        List<int> resSetDup = Task3_RemoveDuplicatesHashSet(dupList);
        sw.Stop();
        long setTime3 = sw.ElapsedMilliseconds;

        Console.WriteLine($"2. Вилучення дублікатів (20k): Список = {listTime3} мс, HashSet = {setTime3} мс");

        // 3. Об'єднання двох наборів по 5 000 елементів
        List<int> set1 = GenerateRandomList(5000, 1, 10000, 123);
        List<int> set2 = GenerateRandomList(5000, 1, 10000, 456);

        sw.Restart();
        List<int> unionListRes = Task4_UnionManualLists(set1, set2);
        sw.Stop();
        long listTime4 = sw.ElapsedMilliseconds;

        sw.Restart();
        List<int> unionSetRes = Task4_UnionHashSet(set1, set2);
        sw.Stop();
        long setTime4 = sw.ElapsedMilliseconds;

        Console.WriteLine($"3. Об'єднання наборів (5k + 5k): Список = {listTime4} мс, HashSet = {setTime4} мс");
    }
}