using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageCombiner.App.ViewModels;
using ImageCombiner.App.Views;

namespace ImageCombiner.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(),
                    Width = MainWindow.DefaultWidth,
                    MinWidth = MainWindow.DefaultWidth,
                    Height = MainWindow.DefaultHeight,
                    MinHeight = MainWindow.DefaultHeight,
                    Title = nameof(ImageCombiner)
                };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel()
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}