using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CodeJam;
using CodeJam.Threading;
using ImageCombiner.Core.Models;

namespace ImageCombiner.App.Infrastructure;

public sealed class AvaloniaFileProvider : IFileProvider
{
    private readonly IReadOnlyList<IStorageFile> _inputFiles;
    private readonly IStorageFile _outputFile;

    public AvaloniaFileProvider(IReadOnlyList<IStorageFile> inputFiles, IStorageFile outputFile)
    {
        Code.NotNullNorEmptyAndItemNotNull(inputFiles, nameof(inputFiles));
        Code.NotNull(outputFile, nameof(outputFile));

        _inputFiles = inputFiles;
        _outputFile = outputFile;
    }

    public async Task<DisposableList<Stream>> GetInputStreamsAsync(CancellationToken ct = default)
    {
        using var inputStreams = (await _inputFiles.Select(f => f.OpenReadAsync()).WhenAll()).ToDisposableList();

        var tempFileStreams = await inputStreams.Select(rawStream => CopyToTempFile(rawStream, ct)).WhenAll();

        return tempFileStreams.ToDisposableList();
    }

    public Task<Stream> GetOutputStreamAsync(CancellationToken ct = default) => _outputFile.OpenWriteAsync();

    // Web Avalonia streams cannot be read twice or be rewound
    // So we'll make temp copies of input files
    private static async Task<Stream> CopyToTempFile(Stream inputFile, CancellationToken ct = default)
    {
        var tempFileStream = new FileStream(GetTempFileName(), 
            FileMode.CreateNew, 
            FileAccess.ReadWrite, 
            FileShare.Read, 
            4096, 
            FileOptions.DeleteOnClose);
        
        await inputFile.CopyToAsync(tempFileStream, ct);
        tempFileStream.Seek(0, SeekOrigin.Begin);

        return tempFileStream;
        
        string GetTempFileName() => Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.image-combiner.tmp");
    }
}