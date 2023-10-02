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
        var inputStreams = await _inputFiles.Select(f => f.OpenReadAsync()).WhenAll();

        return inputStreams.ToDisposableList();
    }

    public Task<Stream> GetOutputStreamAsync(CancellationToken ct = default) => _outputFile.OpenWriteAsync();
}