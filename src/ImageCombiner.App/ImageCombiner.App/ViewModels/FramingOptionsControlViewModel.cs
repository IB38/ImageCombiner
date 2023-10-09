using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ImageCombiner.App.Infrastructure;
using ImageCombiner.Core.Models;
using ReactiveUI;

namespace ImageCombiner.App.ViewModels;

public class FramingOptionsControlViewModel : ViewModelBase
{
    public List<EnumDisplayWrapper<FrameColor>> SelectableFrameColors { get; } = EnumDisplayWrapper<FrameColor>.WrapAllValues();

    public FrameColor FrameColor
    {
        get => _frameColor;
        set => this.RaiseAndSetIfChanged(ref _frameColor, value);
    }
    
    [Range(0, 100)]
    public int FrameThickness
    {
        get => _frameThickness;
        set => this.RaiseAndSetIfChanged(ref _frameThickness, value);
    }

    private FrameColor _frameColor;
    private int _frameThickness;

    public FramingOptionsControlViewModel()
    {
        FrameColor = FrameColor.Black;
    }

    public FramingOptions BuildFramingOptions()
    {
        return new FramingOptions(FrameThickness, FrameColor);
    }
}