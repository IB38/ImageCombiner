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

    private async void CombineButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await ElementHelper.WrapWithDisable(sender, async () =>
        {
            Console.WriteLine("Combining started");
            var inputFiles = FileListControl.Files;

            Code.NotNull(inputFiles, nameof(FileListControl.Files));
            if (inputFiles.Count <= 0)
            {
                Console.WriteLine("Can't combine, no files added");
                return;
            }

            var outputFile = await ShowOutputFilePicker();
            if (outputFile is null)
            {
                Console.WriteLine("No file selected, combining cancelled");
                return;
            }

            await ViewModel.CombineAsync(inputFiles, 
                outputFile, 
                CombinationTypeSelectorControl.SelectedCombinationType,
                FramingOptionsControl.FramingOptions);
            
            Console.WriteLine($"Saved combiner result to file '{outputFile.Name}'");
        });
    }

    private async Task<IStorageFile?> ShowOutputFilePicker()
    {
        Console.WriteLine($"Selecting output file");
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this)!;

        // Start async operation to open the dialog.
        var outputFile = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Select destination for resulting JPEG file",
            DefaultExtension = "jpeg",
            FileTypeChoices = new[] { FilePickerFileTypes.ImageJpg },
            SuggestedFileName = $"{DateTime.Now:yyyy-M-d_HH-mm-ss}",
            ShowOverwritePrompt = true
        });

        Console.WriteLine($"Result file picked {outputFile?.Name ?? "null"}");
        return outputFile;
    }
}