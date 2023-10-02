using System;
using System.Collections.ObjectModel;
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
        set
        {
            var oldFiles = _files;
            this.RaiseAndSetIfChanged(ref _files, value);

            if(!ReferenceEquals(oldFiles, value))
                DisposableHelper.SafeDisposeCollection(oldFiles, true);
        }
    }

    private bool _hasFiles;
    private ObservableCollection<IStorageFile> _files = new();
}