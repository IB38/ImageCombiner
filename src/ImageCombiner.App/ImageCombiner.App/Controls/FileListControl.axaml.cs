using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using DynamicData;
using ImageCombiner.App.Infrastructure;
using ImageCombiner.App.ViewModels;
using ImageCombiner.Core.Models;
using ReactiveUI;
using Console = System.Console;

namespace ImageCombiner.App.Controls;

public partial class FileListControl : UserControl
{
    private FileListControlViewModel ViewModel => DataContext as  FileListControlViewModel 
                                       ?? throw new InvalidOperationException($"Unexpected DataContext type {DataContext?.GetType().Name ?? "null"}");
    
    public static readonly DirectProperty<FileListControl, bool> HasFilesProperty =
        AvaloniaProperty.RegisterDirect<FileListControl, bool>(
            nameof(HasFiles),
            o => o.HasFiles);
    
    public bool HasFiles
    {
        get => _hasFiles;
        set
        {
            SetAndRaise(HasFilesProperty, ref _hasFiles, value);
            ViewModel.HasFiles = value;
        }
    }

    public ObservableCollection<IStorageFile> Files => ViewModel.Files;

    private bool _hasFiles;
    
    public FileListControl()
    {
        InitializeComponent();
        DataContext = new FileListControlViewModel();
        
        // Magic
        // Binding ListBox.ItemCount to HasFiles with coverter
        var itemCountProp = FileListBox.GetObservable(ListBox.ItemCountProperty, count => count > 0);
        itemCountProp.BindTo(this, c => c.HasFiles);
        
        FileListBox.AddHandler(DragDrop.DropEvent, OnFileDropped);
    }

    private async void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await ElementHelper.WrapWithDisable(sender, async () =>
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
                ViewModel.Files.AddRange(files);
        });
    }

    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            Console.WriteLine("Resetting file list");
            ViewModel.Files.Clear();
        });
    }

    private void RemoveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            var removed = IfValidFileSelected((int idx) =>
            {
                Console.WriteLine($"Removing file '{idx}' from the list");
                ViewModel.Files.RemoveAt(idx);
            });

            if(!removed) 
                Console.WriteLine("Can't remove file, list box has no active selection");
        });
    }

    private void MoveUpButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            MoveSelectedListElem((idx) => idx - 1);
        });
    }

    private void MoveDownButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            MoveSelectedListElem((idx) => idx + 1);
        });
    }

    private void MoveTopButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            MoveSelectedListElem((idx) => 0);
        });
    }

    private void MoveBottomButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            MoveSelectedListElem((idx) => ViewModel.Files.Count - 1);
        });
    }
    
    private bool IfValidFileSelected(Action<int> functionBody)
    {
        var selectedIndex = FileListBox.SelectedIndex;
        if (selectedIndex < 0) 
            return false;
        
        functionBody(selectedIndex);
        return true;

    }

    private void ReverseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            ViewModel.ReverseFileOrder();
        });
    }

    private void MoveSelectedListElem(Func<int, int> newIdxFunc)
    {
        if (ViewModel.Files.Count > 0)
        {
            IfValidFileSelected((int idx) =>
            {
                var newIdx = newIdxFunc(idx);
                if (newIdx < 0 || newIdx >= ViewModel.Files.Count)
                {
                    Console.WriteLine($"Can't move file to position '{newIdx}', index is out of bounds");
                    return;
                }
                
                Console.WriteLine($"Moving file '{idx}' to position '{newIdx}'...");
                ViewModel.Files.Move(idx, newIdx);
                // Keep the element selected
                FileListBox.SelectedIndex = newIdx;
            });
        }
        else
        {
            Console.WriteLine("Can't move file, file collection is empty");
        }
    }

    private async void OnFileDropped(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            e.DragEffects = DragDropEffects.Copy;
            
            var storageItems = e.Data.GetFiles() ?? new List<IStorageItem>();

            foreach (var item in storageItems)
            {
                switch (item)
                {
                    case IStorageFile storageFile:
                        Console.WriteLine($"File '{storageFile.Name}' was dropped");
                        ViewModel.Files.Add(storageFile);
                        break;
                    case IStorageFolder storageFolder:
                        Console.WriteLine($"Folder '{storageFolder.Name}' was dropped");
                        await foreach (var folderItem in storageFolder.GetItemsAsync())
                        {
                            // Only files within folder itself, not recursively
                            if(folderItem is IStorageFile folderFile)
                                ViewModel.Files.Add(folderFile);
                        }

                        break;
                    default:
                        return;
                }
            }
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }
}