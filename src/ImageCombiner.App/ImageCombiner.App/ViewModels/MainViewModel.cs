using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Platform.Storage;
using DynamicData;
using ImageCombiner.Core.Models;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    public bool InputSelected
    {
        get => _inputSelected;
        set => this.RaiseAndSetIfChanged(ref _inputSelected, value);
    }
    
    public bool OutputSelected
    {
        get => _outputSelected;
        set => this.RaiseAndSetIfChanged(ref _outputSelected, value);
    }
    
    public ObservableCollection<string> InputFileNames {        
        get => _inputFileNames;
        set => this.RaiseAndSetIfChanged(ref _inputFileNames, value);
    }

    public string? OutputFileName
    {
        get => _outputFileName;
        set => this.RaiseAndSetIfChanged(ref _outputFileName, value);
    }

    public DisposableList<IStorageFile> InputFiles
    {
        get => _inputFiles;
        set
        {
            var oldFiles = _inputFiles;
            this.RaiseAndSetIfChanged(ref _inputFiles, value);
            oldFiles?.Dispose();
            
            UpdateInputProperties();
        }
    }

    public IStorageFile? OutputFile
    {
        get => _outputFile;
        set
        {
            var oldFile = _outputFile;
            this.RaiseAndSetIfChanged(ref _outputFile, value);
            oldFile?.Dispose();
            
            UpdateOutputProperties();
        }
    }

    private DisposableList<IStorageFile> _inputFiles = new();
    private IStorageFile? _outputFile;

    private ObservableCollection<string> _inputFileNames = new();
    private string? _outputFileName;
    private bool _inputSelected;
    private bool _outputSelected;

    private void UpdateInputProperties()
    {
        InputSelected = _inputFiles.Any();
        InputFileNames.Clear();
        InputFileNames.AddRange(_inputFiles.Select(f => f.Name));
    }
    
    private void UpdateOutputProperties()
    {
        OutputSelected = _outputFile != null;
        OutputFileName = _outputFile?.Name;
    }
}