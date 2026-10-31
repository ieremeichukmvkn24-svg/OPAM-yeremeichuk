using System;

class Program
{
    static void Main()
    {
        // Параметри варіанта
        int N = 9;
        int K = 17; // Номер першої літери імені 'М' в абетці
        int a = 0;   // b = 1 -> 16 випадкових чисел у діапазоні [0; 5]
        int b = 1;   // a = 0 -> елементи, більші за (K % 5) = (17 % 5) = 2
        int c = 4;   // c = 4 -> поміняти місцями першу й другу половини масиву

        Console.WriteLine($"N = {N}, K = {K}, a = {a}, b = {b}, c = {c}\n");

        // Ініціалізація генератора згідно з N = 9
        Random rnd = new Random(N);

        // Генерація масиву (16 випадкових чисел у діапазоні [0; 5])
        int[] arr = GenerateArray(16, 0, 5, rnd);

        Console.WriteLine("=== ЗАВДАННЯ 1: Агрегація ===");
        Console.WriteLine("Масив: " + string.Join(", ", arr));
        RunTask1(arr);

        Console.WriteLine("\n=== ЗАВДАННЯ 2: Фільтрація ===");
        // Критерій a = 0: більші за (K % 5) = 17 % 5 = 2
        int threshold = K % 5;
        int[] filtered = RunTask2(arr, threshold);
        Console.WriteLine($"Елементи, більші за {threshold}: " + string.Join(", ", filtered));

        Console.WriteLine("\n=== ЗАВДАННЯ 3: Пошук найдовшої серії ===");
        RunTask3(arr);

        Console.WriteLine("\n=== ЗАВДАННЯ 4: Перестановка на місці (поміняти половини) ===");
        Console.WriteLine("До перестановки: " + string.Join(", ", arr));
        RunTask4(arr);
        Console.WriteLine("Після перестановки: " + string.Join(", ", arr));

        Console.WriteLine("\n=== ЗАВДАННЯ 5: Матриця 5 × 4 ===");
        // Матриця 5 рядків, 4 стовпці (з таблиці B для b = 1)
        int[,] matrix = GenerateMatrix(5, 4, 0, 5, rnd);
        RunTask5(matrix);
    }

    // Генерація масиву
    static int[] GenerateArray(int size, int min, int max, Random rnd)
    {
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = rnd.Next(min, max + 1);
        }
        return array;
    }

    // Генерація матриці
    static int[,] GenerateMatrix(int rows, int cols, int min, int max, Random rnd)
    {
        int[,] mat = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                mat[i, j] = rnd.Next(min, max + 1);
            }
        }
        return mat;
    }

    // ==========================================
    // ЗАВДАННЯ 1. Агрегація за один прохід
    // ==========================================
    static void RunTask1(int[] arr)
    {
        if (arr == null || arr.Length == 0)
        {
            Console.WriteLine("Попередження: Масив порожній. Результати агрегації не визначені.");
            return;
        }

        int sum = 0;
        int min = arr[0];
        int max = arr[0];
        int minIndex = 0;
        int maxIndex = 0;
        int zeroCount = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            int val = arr[i];
            sum += val;

            if (val < min)
            {
                min = val;
                minIndex = i;
            }

            if (val > max)
            {
                max = val;
                maxIndex = i; // Перше входження при рівних максимальних значеннях
            }

            if (val == 0)
            {
                zeroCount++;
            }
        }

        double average = (double)sum / arr.Length;

        Console.WriteLine($"- Сума: {sum}");
        Console.WriteLine($"- Середнє арифметичне: {average:F2}");
        Console.WriteLine($"- Мінімум: {min} (індекс {minIndex})");
        Console.WriteLine($"- Максимум: {max} (індекс {maxIndex})");
        Console.WriteLine($"- Кількість нулів: {zeroCount}");
    }

    // ==========================================
    // ЗАВДАННЯ 2. Фільтрація (точний розмір масиву)
    // ==========================================
    static int[] RunTask2(int[] arr, int threshold)
    {
        if (arr == null || arr.Length == 0)
        {
            return new int[0];
        }

        // 1-й прохід: підрахунок кількості елементів, що задовольняють критерій
        int count = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] > threshold)
            {
                count++;
            }
        }

        // Створення масиву точного розміру
        int[] result = new int[count];

        // 2-й прохід: заповнення нового масиву
        int index = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] > threshold)
            {
                result[index] = arr[i];
                index++;
            }
        }

        return result;
    }

    // ==========================================
    // ЗАВДАННЯ 3. Пошук найдовшої серії однакових елементів
    // ==========================================
    static void RunTask3(int[] arr)
    {
        if (arr == null || arr.Length == 0)
        {
            Console.WriteLine("Масив порожній, серій немає.");
            return;
        }

        int bestVal = arr[0];
        int bestLen = 1;
        int bestStartIndex = 0;

        int currentVal = arr[0];
        int currentLen = 1;
        int currentStartIndex = 0;

        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] == currentVal)
            {
                currentLen++;
            }
            else
            {
                // Перевірка завершеної серії
                if (currentLen > bestLen)
                {
                    bestLen = currentLen;
                    bestVal = currentVal;
                    bestStartIndex = currentStartIndex;
                }
                // Початок нової серії
                currentVal = arr[i];
                currentLen = 1;
                currentStartIndex = i;
            }
        }

        // Перевірка останньої серії (що закінчується на останньому елементі масиву)
        if (currentLen > bestLen)
        {
            bestLen = currentLen;
            bestVal = currentVal;
            bestStartIndex = currentStartIndex;
        }

        Console.WriteLine($"Значення серії: {bestVal}, Довжина: {bestLen}, Індекс початку: {bestStartIndex}");
    }

    // ==========================================
    // ЗАВДАННЯ 4. Перестановка на місці (поміняти половини)
    // ==========================================
    static void RunTask4(int[] arr)
    {
        if (arr == null || arr.Length <= 1) return;

        int mid = arr.Length / 2;
        int offset = arr.Length % 2; // Якщо непарна довжина, центральний елемент залишається на місці

        for (int i = 0; i < mid; i++)
        {
            int targetIdx = i + mid + offset;
            // Обмін елементів першої та другої половин без використання другого масиву
            int temp = arr[i];
            arr[i] = arr[targetIdx];
            arr[targetIdx] = temp;
        }
    }

    // ==========================================
    // ЗАВДАННЯ 5. Обробка матриці за рядками та стовпцями
    // ==========================================
    static void RunTask5(int[,] mat)
    {
        int rows = mat.GetLength(0);
        int cols = mat.GetLength(1);

        Console.WriteLine("Матриця 5 × 4:");
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{mat[i, j],4} ");
            }
            Console.WriteLine();
        }

        // 1. Сума кожного рядка
        Console.WriteLine("\nСуми рядків:");
        int maxSum = int.MinValue;
        int maxRowIndex = -1;

        for (int i = 0; i < rows; i++)
        {
            int rowSum = 0;
            for (int j = 0; j < cols; j++)
            {
                rowSum += mat[i, j];
            }
            Console.WriteLine($"Рядок {i}: сума = {rowSum}");

            if (rowSum > maxSum)
            {
                maxSum = rowSum;
                maxRowIndex = i;
            }
        }
        Console.WriteLine($"-> Рядок з найбільшою сумою: {maxRowIndex} (сума = {maxSum})");

        // 2. Максимум кожного стовпця
        Console.WriteLine("\nМаксимуми стовпців:");
        for (int j = 0; j < cols; j++)
        {
            int colMax = mat[0, j];
            for (int i = 1; i < rows; i++)
            {
                if (mat[i, j] > colMax)
                {
                    colMax = mat[i, j];
                }
            }
            Console.WriteLine($"Стовпець {j}: максимум = {colMax}");
        }
    }
}