using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CodeJam;
using DynamicData;
using ImageCombiner.App.Infrastructure;
using ImageCombiner.App.ViewModels;
using ImageCombiner.Core.Models;

namespace ImageCombiner.App.Views;

public partial class MainView : UserControl
{
    private MainViewModel ViewModel => DataContext as  MainViewModel 
                                       ?? throw new InvalidOperationException($"Unexpected DataContext type {DataContext?.GetType().Name ?? "null"}");
    public MainView()
    {
        InitializeComponent();
    }

    private async void ImageSelectorButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await WrapWithDisable(sender, async () =>
        {
            Console.WriteLine($"Selecting input files");
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this)!;

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select images to combine",
                AllowMultiple = true,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            Console.WriteLine($"Picked {files.Count} input files");
            if (files.Any())
                ViewModel.InputFiles = files.ToDisposableList();
            
        });
    }

    private async void ResultSaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await WrapWithDisable(sender, async () =>
        {
            Console.WriteLine($"Selecting output file");
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this)!;

            // Start async operation to open the dialog.
            var outputFile = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Select result destination",
                DefaultExtension = "jpeg",
                FileTypeChoices = new[] { FilePickerFileTypes.ImageJpg },
                SuggestedFileName = $"{DateTime.Now:yyyy-M-d_HH-mm-ss}",
                ShowOverwritePrompt = true
            });

            Console.WriteLine($"Result file picked {outputFile?.Name ?? "null"}");
            if (outputFile != null)
                ViewModel.OutputFile = outputFile;
        });

    }

    private async void CombineButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await WrapWithDisable(sender, async () =>
        {
            Console.WriteLine("Combining started");
            
            Code.NotNullNorEmpty(ViewModel.InputFiles, nameof(ViewModel.InputFiles));
            Code.NotNull(ViewModel.OutputFile, nameof(ViewModel.OutputFile));
            
            var fileProvider = new AvaloniaFileProvider(ViewModel.InputFiles, ViewModel.OutputFile);
            
            var combiner = new Core.ImageCombiner();
            var input = new CombinerInput(fileProvider);

            await combiner.CombineAsync(input);
            Console.WriteLine($"Finished combining, result file - {ViewModel.OutputFile.Name}");
        });
    }

    private static async Task WrapWithDisable(object? sender, Func<Task> asyncFunctionBody)
    {
        Code.NotNull(sender, nameof(sender));
        var senderElement = sender as InputElement ??
                           throw new InvalidOperationException(
                               $"Unexpected sender type {sender?.GetType().Name ?? "null"}");

        var initiallyEnabled = senderElement.IsEnabled;
        senderElement.IsEnabled = false;
        
        try
        {
            await asyncFunctionBody();
        }
        finally
        {
            senderElement.IsEnabled = initiallyEnabled;
        }
    }
}