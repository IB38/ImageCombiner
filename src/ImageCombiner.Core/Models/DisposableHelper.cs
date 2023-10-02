using CodeJam.Collections;

namespace ImageCombiner.Core.Models;

public static class DisposableHelper
{
    public static void SafeDisposeCollection(IEnumerable<IDisposable> collection, bool silenceExceptions = false)
    {
        var exceptions = new List<Exception>();

        foreach (var elem in collection.EmptyIfNull())
        {
            try
            {
                elem?.Dispose();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }

            if (silenceExceptions)
                return;
            
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