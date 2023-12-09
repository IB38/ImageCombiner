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
    public int MaxResolution 
    {         
        get => _maxResolution;
        set => this.RaiseAndSetIfChanged(ref _maxResolution, value);
    }

    private int _maxResolution;
    private readonly Core.ImageCombiner _combiner = new();

    public async Task CombineAsync(IReadOnlyList<IStorageFile> inputFiles, 
        IStorageFile outputFile,
        CombinationType combinationType,
        SizeMatchingType sizeMatchingType,
        FramingOptions? fo = null,
        CancellationToken ct = default)
    {
        if (inputFiles.Count <= 0)
        {
            Console.WriteLine("No input files to combine");
            return;
        }

        var combinerInput = new CombinerInput(new AvaloniaFileProvider(inputFiles, outputFile))
        {
            FramingOptions = fo,
            CombinationType = combinationType,
            SizeMatchingType = sizeMatchingType,
            MaxResolution = MaxResolution
        };

        await _combiner.CombineAsync(combinerInput, ct);
    }
}