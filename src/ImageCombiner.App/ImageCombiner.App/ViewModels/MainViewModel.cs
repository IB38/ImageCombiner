using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using DynamicData;
using ImageCombiner.App.Infrastructure;
using ImageCombiner.Core.Models;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly Core.ImageCombiner _combiner = new();

    public async Task CombineAsync(IReadOnlyList<IStorageFile> inputFiles, 
        IStorageFile outputFile, 
        CancellationToken ct = default)
    {
        if (inputFiles.Count <= 0)
        {
            Console.WriteLine("No input files to combine");
            return;
        }

        var combinerInput = new CombinerInput(new AvaloniaFileProvider(inputFiles, outputFile));

        await _combiner.CombineAsync(combinerInput, ct);
    }
}