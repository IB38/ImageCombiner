using ImageCombiner.Core.Models;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public class SizeMatchingTypeSelectorViewModel : ReactiveObject
{
    public SizeMatchingType SizeMatchingType
    {
        get => _ct;
        set => this.RaiseAndSetIfChanged(ref _ct, value);
    }
    
    private SizeMatchingType _ct;
}