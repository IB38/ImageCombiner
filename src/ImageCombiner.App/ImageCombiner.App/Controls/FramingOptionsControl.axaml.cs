using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ImageCombiner.App.ViewModels;
using ImageCombiner.Core.Models;

namespace ImageCombiner.App.Controls;

public partial class FramingOptionsControl : UserControl
{
    private FramingOptionsControlViewModel ViewModel => DataContext as  FramingOptionsControlViewModel 
                                                        ?? throw new InvalidOperationException($"Unexpected DataContext type {DataContext?.GetType().Name ?? "null"}");

    public FramingOptions FramingOptions => ViewModel.BuildFramingOptions();
    
    public FramingOptionsControl()
    {
        InitializeComponent();
        DataContext = new FramingOptionsControlViewModel();
        
    }
}