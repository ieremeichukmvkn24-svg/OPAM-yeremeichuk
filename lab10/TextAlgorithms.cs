using System.Text;

static class TextAlgorithms
{
    // Для кожного початку перевіряємо кожного кандидата заново.
    // Після першого повтору всі довші підрядки з цим початком теж містять повтор.
    public static string Naive(string text)
    {
        int start = 0, best = 0;
        for (int i = 0; i < text.Length; i++)
            for (int end = i; end < text.Length; end++)
            {
                bool unique = true;
                for (int j = i; j <= end && unique; j++)
                    for (int k = i; k < j; k++)
                        if (text[k] == text[j]) { unique = false; break; }
                if (!unique) break;
                if (end - i + 1 > best) { start = i; best = end - i + 1; }
            }
        return text.Substring(start, best);
    }
    public static string Window(string text)
    {
        int left = 0, start = 0, best = 0;
        var seen = new HashSet<char>();
        for (int right = 0; right < text.Length; right++)
        {
            while (!seen.Add(text[right])) seen.Remove(text[left++]);
            if (right - left + 1 > best) { start = left; best = right - left + 1; }
        }
        return text.Substring(start, best);
    }
    public static void Agree(string text)
    {
        if (!string.Equals(Naive(text), Window(text), StringComparison.Ordinal)) throw new Exception("Різні підрядки");
    }
    public static int IndexOf(string text, string pattern, out int comparisons)
    {
        comparisons = 0;
        for (int i = 0; i <= text.Length - pattern.Length; i++)
        {
            int j = 0;
            while (j < pattern.Length)
            {
                comparisons++;
                if (text[i + j] != pattern[j]) break;
                j++;
            }
            if (j == pattern.Length) return i;
        }
        return -1;
    }
}
