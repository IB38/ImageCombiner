using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ImageCombiner.App.ViewModels;
using ImageCombiner.Core.Models;

namespace ImageCombiner.App.Controls;

public partial class SizeMatchingTypeSelectorControl : UserControl
{
    public SizeMatchingType SelectedSizeMatchingType => ViewModel.SizeMatchingType;
    
    private SizeMatchingTypeSelectorViewModel ViewModel => DataContext as  SizeMatchingTypeSelectorViewModel 
                                                          ?? throw new InvalidOperationException($"Unexpected DataContext type {DataContext?.GetType().Name ?? "null"}");
    
    public SizeMatchingTypeSelectorControl()
    {
        InitializeComponent();
        
        DataContext = new SizeMatchingTypeSelectorViewModel();
    }

    private void DisabledButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SizeMatchingType = SizeMatchingType.Disabled;
    }

    private void StretchButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SizeMatchingType = SizeMatchingType.Stretch;
    }

    private void ShrinkButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.SizeMatchingType = SizeMatchingType.Shrink;
    }
}