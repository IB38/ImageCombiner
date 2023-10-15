using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImageCombiner.App.Infrastructure;
using ImageCombiner.Core.Models;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public class FramingOptionsControlViewModel : ViewModelBase
{
    public List<EnumDisplayWrapper<FrameColor>> SelectableFrameColors { get; } = EnumDisplayWrapper<FrameColor>.WrapAllValues();

    public FrameColor? SelectedFrameColor
    {
        get => _selectedFrameColor;
        set => this.RaiseAndSetIfChanged(ref _selectedFrameColor, value);
    }
    
    [Range(0, 100)]
    public int SelectedFrameThickness
    {
        get => _selectedFrameThickness;
        set => this.RaiseAndSetIfChanged(ref _selectedFrameThickness, value);
    }

    private FrameColor? _selectedFrameColor;
    private int _selectedFrameThickness;

    public FramingOptionsControlViewModel()
    {
        SelectedFrameColor = FrameColor.Black;
    }

    public FramingOptions BuildFramingOptions()
    {
        return new FramingOptions(SelectedFrameThickness, SelectedFrameColor ?? FrameColor.Black);
    }
}