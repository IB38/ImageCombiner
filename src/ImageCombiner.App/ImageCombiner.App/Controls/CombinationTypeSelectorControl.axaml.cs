using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ImageCombiner.App.ViewModels;
using ImageCombiner.Core.Models;

namespace ImageCombiner.App.Controls;

public partial class CombinationTypeSelectorControl : UserControl
{
    public CombinationType SelectedCombinationType => ViewModel.CombinationType;
    
    private CombinationTypeSelectorViewModel ViewModel => DataContext as  CombinationTypeSelectorViewModel 
                                                  ?? throw new InvalidOperationException($"Unexpected DataContext type {DataContext?.GetType().Name ?? "null"}");
    
    public CombinationTypeSelectorControl()
    {
        InitializeComponent();
        
        DataContext = new CombinationTypeSelectorViewModel();
    }

    private void VerticalButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.CombinationType = CombinationType.Vertical;
    }

    private void HorizontalButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.CombinationType = CombinationType.Horizontal;
    }
}