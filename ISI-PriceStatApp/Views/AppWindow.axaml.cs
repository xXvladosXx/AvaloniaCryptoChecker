using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ISI_PriceStatApp.Views;

public class AppWindow : Window
{
    public AppWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}