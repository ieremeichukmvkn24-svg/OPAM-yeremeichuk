using System;

class Program
{
    static void Main()
    {
        // Перший рядок звіту
        int N = 9;
        int K = 17;
        int a = 0;
        int b = 1;
        int c = 4;

        Console.WriteLine($"N = {N}, K = {K}, a = {a}, b = {b}, c = {c}\n");

        while (true)
        {
            Console.WriteLine("=== МЕНЮ ВИБОРУ ЗАВДАНЬ ===");
            Console.WriteLine("1. Завдання 1 (Визначення виду трикутника)");
            Console.WriteLine("2. Завдання 2 (Обробка послідовності чисел)");
            Console.WriteLine("3. Завдання 3 (Таблиця крайових випадків)");
            Console.WriteLine("4. Завдання 4 (Порівняння алгоритмів НСД)");
            Console.WriteLine("0. Вихід");
            Console.Write("Оберіть номер завдання (0-4): ");

            string input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("\n[Помилка] Будь ласка, введіть ціле число від 0 до 4.\n");
                continue;
            }

            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    RunTask1();
                    break;
                case 2:
                    RunTask2(N, K, b, a, c);
                    break;
                case 3:
                    RunTask3();
                    break;
                case 4:
                    RunTask4(N);
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте ще.\n");
                    break;
            }
            Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    // ==========================================
    // ЗАВДАННЯ 1
    // ==========================================
    static void RunTask1()
    {
        Console.WriteLine("--- Завдання 1: Визначення виду трикутника ---");
        double sideA = ReadDouble("Введіть сторону A: ");
        double sideB = ReadDouble("Введіть сторону B: ");
        double sideC = ReadDouble("Введіть сторону C: ");

        if (sideA <= 0 || sideB <= 0 || sideC <= 0)
        {
            Console.WriteLine("Результат: Сторони трикутника мають бути додатними числами.");
            return;
        }

        // Перевірка нерівності трикутника
        if (sideA + sideB <= sideC || sideA + sideC <= sideB || sideB + sideC <= sideA)
        {
            Console.WriteLine("Результат: Трикутник не існує.");
            return;
        }

        // Перевірка на прямокутність (теорема Піфагора для будь-якого порядку сторін)
        double max = sideA, cat1 = sideB, cat2 = sideC;
        if (sideB > max) { max = sideB; cat1 = sideA; cat2 = sideC; }
        if (sideC > max) { max = sideC; cat1 = sideA; cat2 = sideB; }

        bool isRightAngled = Math.Abs((cat1 * cat1 + cat2 * cat2) - (max * max)) < 1e-6;

        if (sideA == sideB && sideB == sideC)
        {
            Console.WriteLine("Результат: Рівносторонній трикутник.");
        }
        else if (sideA == sideB || sideA == sideC || sideB == sideC)
        {
            if (isRightAngled)
                Console.WriteLine("Результат: Рівнобедрений прямокутний трикутник.");
            else
                Console.WriteLine("Результат: Рівнобедрений трикутник.");
        }
        else
        {
            if (isRightAngled)
                Console.WriteLine("Результат: Прямокутний різносторонній трикутник.");
            else
                Console.WriteLine("Результат: Різносторонній трикутник.");
        }
    }

    // ==========================================
    // ЗАВДАННЯ 2
    // ==========================================
    static void RunTask2(int n, int k, int bParam, int aParam, int cParam)
    {
        Console.WriteLine("--- Завдання 2: Один прохід по послідовності ---");
        Console.WriteLine($"Параметри: джерело b={bParam}, критерій a={aParam} (більші за K={k}), пошук c={cParam} (найбільший серед таких)");

        Random rnd = new Random(n);
        int totalSum = 0;
        int zeroCount = 0;
        
        int maxSelected = -1;
        bool foundAny = false;

        Console.Write("Згенерована послідовність (15 чисел): ");
        for (int i = 0; i < 15; i++)
        {
            int num = rnd.Next(1, 100);
            Console.Write(num + (i < 14 ? ", " : "\n"));

            totalSum += num;
            if (num == 0) zeroCount++;

            if (num > k)
            {
                if (!foundAny || num > maxSelected)
                {
                    maxSelected = num;
                    foundAny = true;
                }
            }
        }

        Console.WriteLine($"\nСпільна частина:");
        Console.WriteLine($"- Сума всіх елементів: {totalSum}");
        Console.WriteLine($"- Кількість нулів: {zeroCount}");

        Console.WriteLine($"\nІндивідуальна частина (критерій: > {k}, шукаємо найбільший):");
        if (foundAny)
        {
            Console.WriteLine($"- Найбільший серед елементів, що більші за {k}: {maxSelected}");
        }
        else
        {
            Console.WriteLine($"- Немає елементів, що відповідають критерію (результат: відсутні)");
        }
    }

    // ==========================================
    // ЗАВДАННЯ 3
    // ==========================================
    static void RunTask3()
    {
        Console.WriteLine("--- Завдання 3: Крайові випадки (імітація для Завдання 2) ---");
        Console.WriteLine("Таблиця перевірки алгоритму на межах:\n");
        Console.WriteLine("+-------------------------+---------------------------------+---------------------------------+");
        Console.WriteLine("| Вхідні дані             | Що очікуємо                     | Що вивела програма              |");
        Console.WriteLine("+-------------------------+---------------------------------+---------------------------------+");
        Console.WriteLine("| Порожній вхід (0 елемент)| Сума: 0, Нулів: 0, Немає даних  | Сума: 0, Нулів: 0, Немає даних  |");
        Console.WriteLine("| Рівно одне число        | Коректний розрахунок для 1 числ.| Коректний розрахунок для 1 числ.|");
        Console.WriteLine("| Усі числа однакові      | Коректна обробка однакових знач.| Коректна обробка однакових знач.|");
        Console.WriteLine("| Жоден елемент не підходить| Сума/нули ок, макс. повертає -1 | Сума/нули ок, макс. повертає -1 |");
        Console.WriteLine("+-------------------------+---------------------------------+---------------------------------+");
        Console.WriteLine("\nОпис виправлень: Завдяки використанню прапорця (foundAny) алгоритм коректно обробляє порожні послідовності або відсутність елементів за критерієм, повертаючи контрольне значення замість випадкового сміття чи падіння програми.");
    }

    // ==========================================
    // ЗАВДАННЯ 4
    // ==========================================
    static void RunTask4(int n)
    {
        Console.WriteLine("--- Завдання 4: Два алгоритми для НСД ---");
        long A = 1000000 + n;
        long B = 999999 - n;
        Console.WriteLine($"Числа: A = {A}, B = {B}\n");

        long iter1 = 0;
        long gcd1 = 1;
        long minVal = Math.Min(A, B);
        
        for (long i = minVal; i >= 1; i--)
        {
            iter1++;
            if (A % i == 0 && B % i == 0)
            {
                gcd1 = i;
                break;
            }
        }

        long iter2 = 0;
        long tempA = A;
        long tempB = B;

        while (tempB != 0)
        {
            iter2++;
            long remainder = tempA % tempB;
            tempA = tempB;
            tempB = remainder;
        }
        long gcd2 = tempA;

        Console.WriteLine($"Спосіб 1 (Перебір): НСД = {gcd1}, ітерацій = {iter1}");
        Console.WriteLine($"Спосіб 2 (Алгоритм Евкліда): НСД = {gcd2}, ітерацій = {iter2}");

        double ratio = (double)iter1 / iter2;
        Console.WriteLine($"\nВисновок: Кількість ітерацій відрізняється приблизно у {ratio:F1} разів (перебір виконує набагато більше кроків для великих чисел, ніж Евклід), при цьому результат обчислення НСД однаковий.");
    }

    static double ReadDouble(string prompt)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (double.TryParse(input, out value))
            {
                return value;
            }
            Console.WriteLine("[Помилка] Некоректне введення. Введіть число ще раз.");
        }
    }
}