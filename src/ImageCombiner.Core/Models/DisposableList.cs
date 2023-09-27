using System.Diagnostics.CodeAnalysis;

namespace ImageCombiner.Core.Models;

public sealed class DisposableList<TDisposable> : List<TDisposable>, IDisposable where TDisposable : class, IDisposable
{
    public void Dispose()
    {
        var exceptions = new List<Exception>();

        foreach (var elem in this)
        {
            try
            {
                elem?.Dispose();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            
            switch (exceptions.Count)
            {
                case <= 0:
                    return;
                case 1:
                    throw exceptions.First();
                default:
                    throw new AggregateException(exceptions);
            }
        }
    }
}

public static class DisposableListHelpers
{
    public static DisposableList<TDisposable> ToDisposableList<TDisposable>(this IEnumerable<TDisposable> source)
        where TDisposable : class, IDisposable
    {
        var resultList = new DisposableList<TDisposable>();
        resultList.AddRange(source);

        return resultList;
    }
}