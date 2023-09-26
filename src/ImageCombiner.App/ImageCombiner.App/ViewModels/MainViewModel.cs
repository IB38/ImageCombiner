using System.Collections.ObjectModel;
using System.IO;
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
    
    public FilePickerResult FilePickerResult { get; }

    public Stream? OutputStream
    {
        get => _outputStream;
        set
        {
            var oldStream = _outputStream;
            this.RaiseAndSetIfChanged(ref _outputStream, value);
            InputSelected = value != null;
            oldStream?.Dispose();
        }
    }

    private Stream? _outputStream;
    private bool _inputSelected;
    private bool _outputSelected;

    public MainViewModel()
    {
        FilePickerResult = new FilePickerResult();
        OutputStream = default;
        
    }
}