using System.Diagnostics.CodeAnalysis;

namespace ImageCombiner.Core.Models;

public sealed class DisposableList<TDisposable> : List<TDisposable>, IDisposable where TDisposable : class, IDisposable
{
    public void Dispose()
    {
        DisposableHelper.SafeDisposeCollection(this, false);
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