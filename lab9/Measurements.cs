using System.Diagnostics;

static class Measurements
{
    public static double Best(Action run)
    {
        run();
        double best = double.MaxValue;
        for (int trial = 0; trial < 3; trial++)
        {
            var timer = Stopwatch.StartNew();
            run();
            timer.Stop();
            best = Math.Min(best, timer.Elapsed.TotalMilliseconds);
        }
        return best;
    }
}
