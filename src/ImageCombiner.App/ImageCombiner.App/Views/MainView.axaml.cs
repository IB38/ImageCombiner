using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using DynamicData;
using ImageCombiner.App.ViewModels;

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
        var senderButton = sender as Button ??
                           throw new InvalidOperationException(
                               $"Unexpected sender type {sender?.GetType().Name ?? "null"}");
        
        senderButton.IsEnabled = false;

        try
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this)!;

            // Start async operation to open the dialog.
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select images to combine",
                AllowMultiple = true,
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });

            Debug.WriteLine($"Picked {files.Count} input files");
            if (files.Any())
            {
                var result = ViewModel.FilePickerResult;
                ViewModel.InputSelected = await result.FillAsync(files);
            }
        }
        finally
        {
            senderButton.IsEnabled = true;
        }
    }

    private async void ResultSaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button ??
                           throw new InvalidOperationException(
                               $"Unexpected sender type {sender?.GetType().Name ?? "null"}");
            
        senderButton.IsEnabled = false;

        try
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this)!;

            // Start async operation to open the dialog.
            var outputFile = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Select result destination", 
                DefaultExtension = "jpeg", 
                FileTypeChoices = new []{ FilePickerFileTypes.ImageJpg }, 
                SuggestedFileName = $"{DateTime.Now}", 
                ShowOverwritePrompt = true
            });

            Debug.WriteLine($"Result file picked {outputFile?.Name ?? "null"}");
            if (outputFile != null)
                ViewModel.OutputStream = await outputFile.OpenWriteAsync();
        }
        finally
        {
            senderButton.IsEnabled = true;
        }
    }

    private void CombineButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("CombineButton clicked ");
    }
}