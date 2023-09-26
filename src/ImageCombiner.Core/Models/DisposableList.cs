namespace ImageCombiner.Core.Models;

public sealed class DisposableList<TDisposable> : List<TDisposable>, IDisposable where TDisposable : IDisposable
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