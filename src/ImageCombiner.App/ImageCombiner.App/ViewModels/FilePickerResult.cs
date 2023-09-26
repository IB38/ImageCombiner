using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public sealed class FilePickerResult : ReactiveObject, IDisposable
{
    public ObservableCollection<string> FileNames { get; private set; } = new ObservableCollection<string>();
    public ObservableCollection<Stream> FileStreams { get; private set; } = new ObservableCollection<Stream>();

    public async Task<bool> FillAsync(IEnumerable<IStorageFile> files)
    {
        Reset();
        
        foreach (var f in files)
        {
            FileNames.Add(f.Name);
            FileStreams.Add(await f.OpenReadAsync());
        }

        return FileStreams.Any();
    }

    public void Dispose()
    {
        Reset();
    }

    private void Reset()
    {
        DisposeStreams();
        FileNames.Clear();
        FileStreams.Clear();
    }

    private void DisposeStreams()
    {
        var exceptions = new List<Exception>();
        foreach (var stream in FileStreams)
        {
            try
            {
                stream?.Dispose();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }
        
        switch (exceptions.Count)
        {
            case <=0: 
                return;
            case 1: 
                throw exceptions.First();
            default: 
                throw new AggregateException(exceptions);
        }
    }
}