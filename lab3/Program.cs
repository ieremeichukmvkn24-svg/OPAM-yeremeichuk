using System;
using System.IO;

class Program
{
    static void Main()
    {
        // Параметри варіанта
        int N = 9;
        int K = 17;
        int a = 0;
        int b = 1;
        int c = 4;

        Console.WriteLine($"N = {N}, K = {K}, a = {a}, b = {b}, c = {c}\n");

        while (true)
        {
            Console.WriteLine("=== МЕНЮ ЛАБОРАТОРНОЇ РОБОТИ №3 ===");
            Console.WriteLine("1. Завдання 1 (Максимальна глибина вкладеності папок)");
            Console.WriteLine("2. Завдання 2 (Максимум масиву через поділ навпіл)");
            Console.WriteLine("3. Завдання 3 (Перебір з поверненням: розмімін 15 грн)");
            Console.WriteLine("4. Завдання 4 (Трасування стека викликів)");
            Console.WriteLine("5. Завдання 5 (Ітераційна версія пошуку максимуму)");
            Console.WriteLine("0. Вихід");
            Console.Write("Оберіть номер завдання (0-5): ");

            string input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("\n[Помилка] Введіть ціле число від 0 до 5.\n");
                continue;
            }

            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    RunTask1();
                    break;
                case 2:
                    RunTask2(N);
                    break;
                case 3:
                    RunTask3();
                    break;
                case 4:
                    RunTask4();
                    break;
                case 5:
                    RunTask5(N);
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Невірний вибір.\n");
                    break;
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    // ==========================================
    // ЗАВДАННЯ 1. Максимальна глибина вкладеності папок
    // ==========================================
    static void RunTask1()
    {
        Console.WriteLine("--- Завдання 1: Максимальна глибина вкладеності папок ---");
        Console.Write("Введіть шлях до каталогу: ");
        string path = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            Console.WriteLine("Повідомлення: Вказана папка не існує або шлях порожній.");
            return;
        }

        int maxDepth = GetMaxFolderDepth(path, 0);
        Console.WriteLine($"Максимальна глибина вкладеності папок відносно кореня: {maxDepth}");
    }

    static int GetMaxFolderDepth(string dirPath, int currentDepth)
    {
        int deepest = currentDepth;

        try
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            foreach (string subDir in subDirs)
            {
                int subDepth = GetMaxFolderDepth(subDir, currentDepth + 1);
                if (subDepth > deepest)
                {
                    deepest = subDepth;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Пропуск папок, до яких немає доступу за прав адміністратора/користувача
        }
        catch (DirectoryNotFoundException)
        {
            // Пропуск зниклих папок
        }

        return deepest;
    }

    // ==========================================
    // ЗАВДАННЯ 2. Максимум масиву через дві половини
    // ==========================================
    static void RunTask2(int n)
    {
        Console.WriteLine("--- Завдання 2: Максимум масиву (поділ навпіл) ---");
        int[] arr = GenerateArray(16, -20, 20, n);
        Console.WriteLine("Масив (16 чисел): " + string.Join(", ", arr));

        int maxDepthRec = 0;
        int maxVal = FindMaxRecursive(arr, 0, arr.Length - 1, 0, ref maxDepthRec);

        double log2N = Math.Log(arr.Length, 2);
        Console.WriteLine($"Максимальний елемент: {maxVal}");
        Console.WriteLine($"Максимальна глибина рекурсії: {maxDepthRec}");
        Console.WriteLine($"Теоретичний логарифм log2({arr.Length}) = {log2N}");
    }

    static int FindMaxRecursive(int[] a, int lo, int hi, int currentDepth, ref int maxDepth)
    {
        if (currentDepth > maxDepth)
        {
            maxDepth = currentDepth;
        }

        // Базовий випадок: залишився один елемент
        if (lo == hi)
        {
            return a[lo];
        }

        int mid = (lo + hi) / 2;
        int leftMax = FindMaxRecursive(a, lo, mid, currentDepth + 1, ref maxDepth);
        int rightMax = FindMaxRecursive(a, mid + 1, hi, currentDepth + 1, ref maxDepth);

        return Math.Max(leftMax, rightMax);
    }

    // ==========================================
    // ЗАВДАННЯ 3. Перебір з поверненням (Розмін 15 грн)
    // ==========================================
    static void RunTask3()
    {
        Console.WriteLine("--- Завдання 3: Способи розміняти 15 грн монетами 1, 2 і 5 ---");
        int targetAmount = 15;
        int[] coins = { 5, 2, 1 }; // Сортуємо за спаданням для зручності
        int totalWays = 0;

        FindCoinCombinations(coins, targetAmount, 0, new int[coins.Length], ref totalWays);
        Console.WriteLine($"Загальна кількість способів розміняти {targetAmount} грн: {totalWays}");
    }

    static void FindCoinCombinations(int[] coins, int remainingAmount, int coinIndex, int[] currentCounts, ref int waysCount)
    {
        // Базовий випадок 1: суму успішно розміняно
        if (remainingAmount == 0)
        {
            Console.WriteLine($"Спосіб #{waysCount + 1}: 5грн x {currentCounts[0]}, 2грн x {currentCounts[1]}, 1грн x {currentCounts[2]}");
            waysCount++;
            return;
        }

        // Базовий випадок 2: сума менша за 0 або закінчилися номінали монет
        if (remainingAmount < 0 || coinIndex >= coins.Length)
        {
            return;
        }

        int currentCoin = coins[coinIndex];
        int maxPossible = remainingAmount / currentCoin;

        // Перебір кількості монет поточного номіналу від максимального до 0
        for (int count = maxPossible; count >= 0; count--)
        {
            currentCounts[coinIndex] = count;
            // Рекурсивний виклик для наступного номіналу монети (backtracking)
            FindCoinCombinations(coins, remainingAmount - count * currentCoin, coinIndex + 1, currentCounts, ref waysCount);
        }
        currentCounts[coinIndex] = 0; // Очищення стану при поверненні
    }

    // ==========================================
    // ЗАВДАННЯ 4. Трасування стека викликів
    // ==========================================
    static void RunTask4()
    {
        Console.WriteLine("--- Завдання 4: Трасування стека викликів на 4 елементах ---");
        int[] smallArr = { 7, -2, 15, 4 };
        Console.WriteLine("Масив: " + string.Join(", ", smallArr));
        Console.WriteLine("Вивід трасування:\n");

        int result = TraceMaxRecursive(smallArr, 0, smallArr.Length - 1, 0);
        Console.WriteLine($"\nРезультат виконання: {result}");
    }

    static int TraceMaxRecursive(int[] a, int lo, int hi, int depth)
    {
        string indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}enter Max({lo}..{hi})");

        if (lo == hi)
        {
            Console.WriteLine($"{indent}  base case -> {a[lo]}");
            Console.WriteLine($"{indent}exit  Max({lo}..{hi}) -> {a[lo]}");
            return a[lo];
        }

        int mid = (lo + hi) / 2;
        int leftMax = TraceMaxRecursive(a, lo, mid, depth + 1);
        int rightMax = TraceMaxRecursive(a, mid + 1, hi, depth + 1);
        int currentMax = Math.Max(leftMax, rightMax);

        Console.WriteLine($"{indent}exit  Max({lo}..{hi}) -> {currentMax}");
        return currentMax;
    }

    // ==========================================
    // ЗАВДАННЯ 5. Ітеративна версія (без рекурсії)
    // ==========================================
    static void RunTask5(int n)
    {
        Console.WriteLine("--- Завдання 5: Ітеративна версія пошуку максимуму ---");
        int[] arr = GenerateArray(16, -20, 20, n);
        Console.WriteLine("Масив: " + string.Join(", ", arr));

        // Пошук максимуму звичайним циклом
        int maxIterative = arr[0];
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] > maxIterative)
            {
                maxIterative = arr[i];
            }
        }

        Console.WriteLine($"Максимум, знайдений ітераційно: {maxIterative}");
    }

    // Допоміжний метод генерації масиву
    static int[] GenerateArray(int size, int min, int max, int seed)
    {
        Random rnd = new Random(seed);
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = rnd.Next(min, max + 1);
        }
        return array;
    }
}