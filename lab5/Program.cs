using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        // Параметри варіанта для Максима Давидюка (N = 9, K = 17)
        int N = 9;
        int K = 17;
        int a = N % 3; // 0
        int b = N % 4; // 1
        int c = N % 5; // 4

        Console.WriteLine($"=== ВАРІАНТ: N = {N}, K = {K}, a = {a}, b = {b}, c = {c} ===");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 1: Збалансованість дужок (на власному стеку CharStack)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 1: Перевірка збалансованості дужок ---");
        string[] testStrings = {
            "(a[b]{c})",
            "([)]",
            ")(",
            "((",
            ""
        };

        foreach (var test in testStrings)
        {
            var (isBalanced, errIndex) = CheckBrackets(test);
            Console.WriteLine($"Рядок: \"{test}\" => Збалансований: {isBalanced}, Індекс помилки: {errIndex}");
        }

        // Свідомо незбалансований тест (17 відкривальних, 16 закривальних)
        string customBadTest = "((((((((((((((((( ))))))))))))))))";
        var (customBalanced, customErrIndex) = CheckBrackets(customBadTest);
        Console.WriteLine($"Незбалансований тест (17 '(' і 16 ')'): Збалансований: {customBalanced}, Індекс помилки: {customErrIndex}");

        // ---------------------------------------------------------
        // ЗАВДАННЯ 2: Зворотний польський запис (операції з рядка b = 1: + - * / та %)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 2: Зворотний польський запис (ЗПЗ) ---");
        string[] rpnTests = {
            "3 4 + 2 *",
            "5 3 -",
            "8 2 /",
            "2 3 4 * +",
            "5 0 /",
            "8 2 * 1 +" // Власний вираз з результатом 17
        };

        foreach (var expr in rpnTests)
        {
            Console.Write($"Вираз: \"{expr}\" => ");
            EvaluateRPN(expr);
        }

        // ---------------------------------------------------------
        // ЗАВДАННЯ 3: Модель обслуговування (c = 4: порціями по K % 3 + 2 = 4 одиниць)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 3: Модель обслуговування заявок (Порції по 4 одиниці) ---");
        RunServiceSimulation(N, K);

        // ---------------------------------------------------------
        // ЗАВДАННЯ 4: Власний стек поверх масиву (IntStack)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- ЗАВДАННЯ 4: Власний стек IntStack та перевірка подвоєння ---");
        int initialCapacity = K % 4 + 2; // 17 % 4 + 2 = 3
        int pushCount = K % 4 + 3;       // 17 % 4 + 3 = 4 (перевірка на 4-му Push)
        
        IntStack customStack = new IntStack(initialCapacity);
        for (int i = 1; i <= pushCount + 1; i++)
        {
            int val = i * 10;
            customStack.Push(val);
            Console.WriteLine($"[Push {val}] -> Count = {customStack.Count}, Capacity = {customStack.Capacity}");
        }
    }

    // =========================================================
    // РЕАЛІЗАЦІЯ ЗАВДАННЯ 1
    // =========================================================
    static (bool, int) CheckBrackets(string input)
    {
        if (string.IsNullOrEmpty(input)) return (true, -1);

        // Використовуємо власний стек символів для Завдання 4
        CharStack stack = new CharStack(10);

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '(' || c == '[' || c == '{')
            {
                stack.Push(c);
            }
            else if (c == ')' || c == ']' || c == '}')
            {
                // Закривальна дужка при порожньому стеку -> помилка на поточному індексі
                if (stack.Count == 0)
                {
                    return (false, i);
                }

                char top = stack.Pop();
                if (!IsMatchingPair(top, c))
                {
                    return (false, i);
                }
            }
        }

        // Якщо стек після кінця рядка непорожній -> помилка на індексі довжини рядка
        if (stack.Count > 0)
        {
            return (false, input.Length);
        }

        return (true, -1);
    }

    static bool IsMatchingPair(char open, char close)
    {
        return (open == '(' && close == ')') ||
               (open == '[' && close == ']') ||
               (open == '{' && close == '}');
    }

    // =========================================================
    // РЕАЛІЗАЦІЯ ЗАВДАННЯ 2
    // =========================================================
    static void EvaluateRPN(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            Console.WriteLine("Помилка: порожній вираз.");
            return;
        }

        string[] tokens = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        Stack<double> stack = new Stack<double>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else
            {
                // Операція (+, -, *, /, %)
                if (stack.Count < 2)
                {
                    Console.WriteLine("Помилка: забагато операторів або бракує операндів.");
                    return;
                }

                double bVal = stack.Pop();
                double aVal = stack.Pop();
                double result = 0;

                switch (token)
                {
                    case "+":
                        result = aVal + bVal;
                        break;
                    case "-":
                        result = aVal - bVal;
                        break;
                    case "*":
                        result = aVal * bVal;
                        break;
                    case "/":
                        if (bVal == 0)
                        {
                            Console.WriteLine("Помилка: ділення на нуль.");
                            return;
                        }
                        result = aVal / bVal;
                        break;
                    case "%":
                        if (bVal == 0)
                        {
                            Console.WriteLine("Помилка: остача від ділення на нуль.");
                            return;
                        }
                        result = aVal % bVal;
                        break;
                    default:
                        Console.WriteLine($"Помилка: невідомий токен '{token}'.");
                        return;
                }
                stack.Push(result);
            }
        }

        if (stack.Count != 1)
        {
            Console.WriteLine("Помилка: забагато чисел у виразі.");
            return;
        }

        Console.WriteLine($"Результат = {stack.Pop()}");
    }

    // =========================================================
    // РЕАЛІЗАЦІЯ ЗАВДАННЯ 3
    // =========================================================
    class RequestItem
    {
        public int Id { get; set; }
        public int RemainingWork { get; set; }
        public int StartTime { get; set; } = -1;
        public int EndTime { get; set; } = -1;
    }

    static void RunServiceSimulation(int n, int k)
    {
        int totalRequests = 8 + k % 5; // 8 + 2 = 10 заявок
        int portionSize = k % 3 + 2;   // 17 % 3 + 2 = 4 одиниць за такт
        Random rnd = new Random(n);

        Queue<RequestItem> queue = new Queue<RequestItem>();

        // Створюємо заявки з випадковою вагою від 1 до 9
        for (int i = 1; i <= totalRequests; i++)
        {
            queue.Enqueue(new RequestItem { Id = i, RemainingWork = rnd.Next(1, 10) });
        }

        int currentTime = 0;
        int maxQueueLength = queue.Count;
        List<RequestItem> completedRequests = new List<RequestItem>();

        Console.WriteLine($"Загальна кількість заявок: {totalRequests}, розмір порції: {portionSize} хв.");

        while (queue.Count > 0)
        {
            if (queue.Count > maxQueueLength)
            {
                maxQueueLength = queue.Count;
            }

            RequestItem currentReq = queue.Dequeue();

            // Якщо заявка починається вперше, фіксуємо час початку
            if (currentReq.StartTime == -1)
            {
                currentReq.StartTime = currentTime;
            }

            int workDone = Math.Min(currentReq.RemainingWork, portionSize);
            currentReq.RemainingWork -= workDone;
            currentTime += workDone;

            if (currentReq.RemainingWork == 0)
            {
                currentReq.EndTime = currentTime;
                completedRequests.Add(currentReq);
            }
            else
            {
                // Повертаємо недороблену заявку в кінець черги
                queue.Enqueue(currentReq);
            }
        }

        // Сортуємо для красивого виведення за ID заявки
        completedRequests.Sort((x, y) => x.Id.CompareTo(y.Id));

        foreach (var req in completedRequests)
        {
            Console.WriteLine($"Заявка #{req.Id} | Початок: {req.StartTime} хв. | Завершення: {req.EndTime} хв.");
        }

        Console.WriteLine($"Максимальна довжина черги за час моделювання: {maxQueueLength}");
    }

    // =========================================================
    // РЕАЛІЗАЦІЯ ЗАВДАННЯ 4: ВЛАСНІ СТЕКИ ПОВЕХ МАСИВУ
    // =========================================================
    class IntStack
    {
        private int[] _array;
        private int _top;

        public IntStack(int capacity)
        {
            if (capacity < 1) capacity = 4;
            _array = new int[capacity];
            _top = 0;
        }

        public int Count => _top;

        public int Capacity => _array.Length;

        public void Push(int item)
        {
            if (_top == _array.Length)
            {
                // Подвоєння масиву
                int newCapacity = _array.Length * 2;
                int[] newArray = new int[newCapacity];
                for (int i = 0; i < _array.Length; i++)
                {
                    newArray[i] = _array[i];
                }
                _array = newArray;
            }

            _array[_top++] = item;
        }

        public int Pop()
        {
            if (_top == 0)
            {
                throw new InvalidOperationException("Стек порожній.");
            }
            return _array[--_top];
        }

        public int Peek()
        {
            if (_top == 0)
            {
                throw new InvalidOperationException("Стек порожній.");
            }
            return _array[_top - 1];
        }
    }

    class CharStack
    {
        private char[] _array;
        private int _top;

        public CharStack(int capacity)
        {
            if (capacity < 1) capacity = 4;
            _array = new char[capacity];
            _top = 0;
        }

        public int Count => _top;

        public void Push(char item)
        {
            if (_top == _array.Length)
            {
                int newCapacity = _array.Length * 2;
                char[] newArray = new char[newCapacity];
                for (int i = 0; i < _array.Length; i++)
                {
                    newArray[i] = _array[i];
                }
                _array = newArray;
            }

            _array[_top++] = item;
        }

        public char Pop()
        {
            if (_top == 0)
            {
                throw new InvalidOperationException("Стек порожній.");
            }
            return _array[--_top];
        }
    }
}