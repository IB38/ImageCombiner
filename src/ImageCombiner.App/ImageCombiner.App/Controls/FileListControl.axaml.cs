using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
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
            if (ViewModel.Files.Count > 0)
            {
                IfValidFileSelected((int idx) =>
                {
                    if (idx > 0)
                    {
                        Console.WriteLine($"Moving file '{idx}' to position '{idx - 1}'");
                        ViewModel.Files.Move(idx, idx - 1);
                    }
                    else
                    {
                        Console.WriteLine($"Can't move file '{idx}' up");
                    }
                });
            }
            else
            {
                Console.WriteLine("Can't move file, file collection is empty");
            }
        });
    }

    private void MoveDownButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementHelper.WrapWithDisable(sender, () =>
        {
            if (ViewModel.Files.Count > 0)
            {
                IfValidFileSelected((int idx) =>
                {
                    if (idx < ViewModel.Files.Count - 1)
                    {
                        Console.WriteLine($"Moving file '{idx}' to position '{idx - 1}'");
                        ViewModel.Files.Move(idx, idx + 1);
                    }
                    else
                    {
                        Console.WriteLine($"Can't move file '{idx}' down");
                    }
                });
            }
            else
            {
                Console.WriteLine("Can't move file, file collection is empty");
            }
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
}