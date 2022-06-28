using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using ISI_PriceStatApp.ViewModels;
using ISI_PriceStatApp.Views;

namespace ISI_PriceStatApp;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new AppWindow()
            {
                DataContext = new AppWindowViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}