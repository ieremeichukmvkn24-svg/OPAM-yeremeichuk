static class Sorts
{
    public static void Selection<T>(T[] data, Func<T, T, int> compare, out long comparisons, out long moves, Action<T[]> trace = null)
    {
        comparisons = moves = 0;
        for (int i = 0; i < data.Length - 1; i++)
        {
            int min = i;
            for (int j = i + 1; j < data.Length; j++)
            {
                comparisons++;
                if (compare(data[j], data[min]) < 0) min = j;
            }
            if (min != i)
            {
                (data[i], data[min]) = (data[min], data[i]);
                moves += 2;
            }
            trace?.Invoke(data);
        }
    }

    public static void Merge<T>(T[] data, Func<T, T, int> compare, out long comparisons, out long moves)
    {
        comparisons = moves = 0;
        T[] buffer = new T[data.Length];
        for (int width = 1; width < data.Length; width *= 2)
        {
            for (int start = 0; start < data.Length; start += 2 * width)
            {
                int middle = Math.Min(start + width, data.Length);
                int end = Math.Min(middle + width, data.Length);
                int left = start, right = middle;
                for (int dest = start; dest < end; dest++)
                {
                    bool takeLeft = right == end;
                    if (left < middle && right < end)
                    {
                        comparisons++;
                        takeLeft = compare(data[left], data[right]) <= 0;
                    }
                    buffer[dest] = takeLeft ? data[left++] : data[right++];
                    moves++;
                }
            }
            Array.Copy(buffer, data, data.Length);
            moves += data.Length;
        }
    }
}
