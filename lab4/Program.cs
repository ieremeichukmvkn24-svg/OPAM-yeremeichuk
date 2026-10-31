using System;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        // Параметри варіанта для Вадима Давидюка (N = 9, K = 17)
        int N = 9;
        int K = 17;
        int a = N % 3; // 0
        int b = N % 4; // 1
        int c = N % 5; // 4

        Console.WriteLine($"=== ВАРІАНТ: N = {N}, K = {K}, a = {a}, b = {b}, c = {c} ===");

        // Генерація основного списку з 20 чисел у діапазоні [-9; 9] за допомогою Random(9)
        List<int> mainList = GenerateList(20, -9, 9, N);
        Console.WriteLine("\nОсновний список:");
        PrintList(mainList);

        // ---------------------------------------------------------
        // ЗАВДАННЯ 1: Побудова нового списку за критерієм (a = 0 -> більші за K % 7 = 3)
        // ---------------------------------------------------------
        int threshold = K % 7; // 17 % 7 = 3
        List<int> filteredList = Task1_Filter(mainList, threshold);
        Console.WriteLine($"\nЗавдання 1 (Елементи більші за {threshold}):");
        PrintList(filteredList);
        Console.WriteLine($"Count: {filteredList.Count}");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 2: Вилучення елементів трьома способами (c = 4 -> менші за середнє арифметичне)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- Завдання 2 (Вилучення елементів, менших за середнє арифметичне) ---");
        
        // 1. Хибний варіант
        List<int> copy1 = new List<int>(mainList);
        Task2_RemoveWrong(copy1);
        Console.WriteLine("Хибний варіант (з початку з RemoveAt):");
        PrintList(copy1);

        // 2. Цикл з кінця
        List<int> copy2 = new List<int>(mainList);
        Task2_RemoveFromEnd(copy2);
        Console.WriteLine("Варіант 2 (цикл з кінця):");
        PrintList(copy2);

        // 3. Новий список
        List<int> copy3 = new List<int>(mainList);
        List<int> copy3Result = Task2_NewList(copy3);
        Console.WriteLine("Варіант 3 (новий список):");
        PrintList(copy3Result);

        // Перевірка ідентичності результатів варіантів 2 і 3
        bool areEqual = ListsAreEqual(copy2, copy3Result);
        Console.WriteLine($"Порівняння результатів варіантів 2 і 3 (мають бути однакові): {areEqual}");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 3: Злиття двох впорядкованих списків
        // ---------------------------------------------------------
        int len1 = 10 + K % 5; // 10 + 2 = 12
        int len2 = 6 + K % 4;  // 6 + 1 = 7
        List<int> sortedList1 = GenerateSortedList(len1, N);
        List<int> sortedList2 = GenerateSortedList(len2, N + 100);

        Console.WriteLine("\n--- Завдання 3 (Злиття списків) ---");
        Console.WriteLine($"Список 1 (довжина {len1}):");
        PrintList(sortedList1);
        Console.WriteLine($"Список 2 (довжина {len2}):");
        PrintList(sortedList2);

        List<int> mergedList = Task3_Merge(sortedList1, sortedList2);
        Console.WriteLine("Злийований впорядкований список:");
        PrintList(mergedList);

        bool isSorted = IsSortedAscending(mergedList);
        Console.WriteLine($"Перевірка впорядкованості злитого списку: {isSorted}");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 4: Розділення списку на два (b = 1 -> парні та непарні)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- Завдання 4 (Розділення на парні та непарні) ---");
        Task4_Split(mainList, out List<int> evenList, out List<int> oddList);
        Console.WriteLine("Парні елементи:");
        PrintList(evenList);
        Console.WriteLine("Непарні елементи:");
        PrintList(oddList);

        bool countMatches = (evenList.Count + oddList.Count) == mainList.Count;
        Console.WriteLine($"Сума Count обох списків дорівнює Count вихідного ({evenList.Count} + {oddList.Count} == {mainList.Count}): {countMatches}");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 5: Замір часу
        // ---------------------------------------------------------
        Console.WriteLine("\n--- Завдання 5 (Вимірювання часу виконання) ---");
        RunBenchmarks();
    }

    // --- Допоміжні методи генерації та виведення ---

    static List<int> GenerateList(int count, int min, int max, int seed)
    {
        Random rnd = new Random(seed);
        List<int> list = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            list.Add(rnd.Next(min, max + 1));
        }
        return list;
    }

    static List<int> GenerateSortedList(int count, int seed)
    {
        Random rnd = new Random(seed);
        List<int> list = new List<int>(count);
        if (count <= 0) return list;

        int current = rnd.Next(1, 10);
        list.Add(current);
        for (int i = 1; i < count; i++)
        {
            current += rnd.Next(1, 10);
            list.Add(current);
        }
        return list;
    }

    static void PrintList(List<int> list)
    {
        Console.WriteLine("[ " + string.Join(", ", list) + " ]");
    }

    static bool ListsAreEqual(List<int> l1, List<int> l2)
    {
        if (l1.Count != l2.Count) return false;
        for (int i = 0; i < l1.Count; i++)
        {
            if (l1[i] != l2[i]) return false;
        }
        return true;
    }

    static bool IsSortedAscending(List<int> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i] > list[i + 1]) return false;
        }
        return true;
    }

    // --- Реалізація завдань ---

    // Завдання 1
    static List<int> Task1_Filter(List<int> source, int threshold)
    {
        List<int> result = new List<int>();
        foreach (var item in source)
        {
            if (item > threshold)
            {
                result.Add(item);
            }
        }
        return result;
    }

    // Завдання 2: 1. Хибний варіант
    static void Task2_RemoveWrong(List<int> list)
    {
        if (list.Count == 0) return;
        double avg = GetAverage(list);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] < avg)
            {
                list.RemoveAt(i); // Хибно: зсуває елементи і пропускає сусідів
            }
        }
    }

    // Завдання 2: 2. Цикл з кінця
    static void Task2_RemoveFromEnd(List<int> list)
    {
        if (list.Count == 0) return;
        double avg = GetAverage(list);

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] < avg)
            {
                list.RemoveAt(i);
            }
        }
    }

    // Завдання 2: 3. Новий список
    static List<int> Task2_NewList(List<int> source)
    {
        List<int> result = new List<int>();
        if (source.Count == 0) return result;
        double avg = GetAverage(source);

        foreach (var item in source)
        {
            if (item >= avg)
            {
                result.Add(item);
            }
        }
        return result;
    }

    static double GetAverage(List<int> list)
    {
        long sum = 0;
        foreach (var item in list) sum += item;
        return (double)sum / list.Count;
    }

    // Завдання 3: Злиття
    static List<int> Task3_Merge(List<int> list1, List<int> list2)
    {
        List<int> result = new List<int>(list1.Count + list2.Count);
        int i = 0, j = 0;

        while (i < list1.Count && j < list2.Count)
        {
            if (list1[i] <= list2[j])
            {
                result.Add(list1[i]);
                i++;
            }
            else
            {
                result.Add(list2[j]);
                j++;
            }
        }

        // Дописуємо залишок першого списку
        while (i < list1.Count)
        {
            result.Add(list1[i]);
            i++;
        }

        // Дописуємо залишок другого списку
        while (j < list2.Count)
        {
            result.Add(list2[j]);
            j++;
        }

        return result;
    }

    // Завдання 4: Розділення (на парні та непарні)
    static void Task4_Split(List<int> source, out List<int> even, out List<int> odd)
    {
        even = new List<int>();
        odd = new List<int>();

        foreach (var item in source)
        {
            if (item % 2 == 0)
            {
                even.Add(item);
            }
            else
            {
                odd.Add(item);
            }
        }
    }

    // Завдання 5: Бенчмарки
    static void RunBenchmarks()
    {
        Stopwatch sw = new Stopwatch();

        // Прогрівальний прогін
        List<int> warmup = new List<int>();
        for (int i = 0; i < 1000; i++) warmup.Add(i);

        // 1. 100 000 x Add без місткості
        sw.Restart();
        List<int> listNoCapacity = new List<int>();
        for (int i = 0; i < 100000; i++)
        {
            listNoCapacity.Add(i);
        }
        sw.Stop();
        Console.WriteLine($"100 000 x Add (без місткості): {sw.ElapsedMilliseconds} мс");

        // 2. 100 000 x Add із заданою місткостю
        sw.Restart();
        List<int> listWithCapacity = new List<int>(100000);
        for (int i = 0; i < 100000; i++)
        {
            listWithCapacity.Add(i);
        }
        sw.Stop();
        Console.WriteLine($"100 000 x Add (з місткостю 100000): {sw.ElapsedMilliseconds} мс");

        // 3. 100 000 x Insert(0, x)
        sw.Restart();
        List<int> listInsert = new List<int>();
        for (int i = 0; i < 100000; i++)
        {
            listInsert.Insert(0, i);
        }
        sw.Stop();
        Console.WriteLine($"100 000 x Insert(0, x): {sw.ElapsedMilliseconds} мс");

        // 4. 1 000 x Contains(999999) у списку з 100 000 елементів
        List<int> searchList = new List<int>(100000);
        for (int i = 0; i < 100000; i++) searchList.Add(i);

        sw.Restart();
        bool dummy = false;
        for (int i = 0; i < 1000; i++)
        {
            dummy = searchList.Contains(999999); // Значення, якого немає
        }
        sw.Stop();
        Console.WriteLine($"1 000 x Contains (неіснуючий елемент): {sw.ElapsedMilliseconds} мс (res: {dummy})");
    }
}