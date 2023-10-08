using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ImageCombiner.App.Infrastructure;
using ImageCombiner.Core.Models;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public class FileListControlViewModel : ViewModelBase
{
    public bool HasFiles 
    {         
        get => _hasFiles;
        set => this.RaiseAndSetIfChanged(ref _hasFiles, value);
    }
    
    public ObservableCollection<IStorageFile> Files
    {
        get => _files;
        set => ReplaceFileCollection(value);
    }

    private bool _hasFiles;
    private ObservableCollection<IStorageFile> _files = new();

    public void ReverseFileOrder() =>
        // Disabling old files disposal because IStorageFile objects stay the same
        ReplaceFileCollection(new ObservableCollection<IStorageFile>(Files.Reverse()), false);
    
    private void ReplaceFileCollection(ObservableCollection<IStorageFile> newFiles, bool disposeOldFiles = true)
    {
        var oldFiles = _files;
        this.RaiseAndSetIfChanged(ref _files, newFiles, nameof(Files));

        if(disposeOldFiles || !ReferenceEquals(oldFiles, newFiles))
            DisposableHelper.SafeDisposeCollection(oldFiles, true);
    }
}