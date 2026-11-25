record struct Interval(int Start, int End);

static class Intervals
{
    public static int Greedy(Interval[] input, Comparison<Interval> order)
    {
        var sorted = (Interval[])input.Clone();
        Array.Sort(sorted, order);
        var accepted = new List<Interval>();
        foreach (var next in sorted)
        {
            bool fits = true;
            foreach (var previous in accepted)
                if (next.Start < previous.End && previous.Start < next.End) { fits = false; break; }
            if (fits) accepted.Add(next);
        }
        return accepted.Count;
    }
    public static readonly Comparison<Interval> Finish = (x, y) => x.End.CompareTo(y.End);
    public static int Exact(Interval[] input, bool prune, out long nodes)
    {
        var sorted = (Interval[])input.Clone();
        Array.Sort(sorted, (x, y) => x.Start.CompareTo(y.Start));
        long visited = 0;
        int best = 0;
        void Visit(int index, int end, int count)
        {
            visited++;
            if (index == sorted.Length) { best = Math.Max(best, count); return; }
            if (prune && count + sorted.Length - index <= best) return;
            if (sorted[index].Start >= end) Visit(index + 1, sorted[index].End, count + 1);
            Visit(index + 1, end, count);
        }
        Visit(0, int.MinValue, 0);
        nodes = visited;
        return best;
    }
}
