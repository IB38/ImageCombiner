namespace ImageCombiner.Core.Models;

public interface IFileProvider
{
    Task<DisposableList<Stream>> GetInputStreamsAsync(CancellationToken ct = default);
    Task<Stream> GetOutputStreamAsync(CancellationToken ct = default);
}