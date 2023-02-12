using System.Diagnostics;

namespace Books.Legacy;

public class ComplicatedPageCalculator
{
    public int CalculateBookPages(Guid bookId)
    {
        var watch = new Stopwatch();
        watch.Start();
        while (true)
        {
            if (watch.ElapsedMilliseconds>5000)
            {
                break;
            }
        }
        return 42;
    }
}