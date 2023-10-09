using ImageCombiner.Core.Models;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public class CombinationTypeSelectorViewModel : ReactiveObject
{
    public CombinationType CombinationType
    {
        get => _ct;
        set => this.RaiseAndSetIfChanged(ref _ct, value);
    }
    
    private CombinationType _ct;
}