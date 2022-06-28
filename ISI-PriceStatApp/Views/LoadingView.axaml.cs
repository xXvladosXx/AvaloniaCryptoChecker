using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ISI_PriceStatApp.Views;

public class LoadingView : UserControl
{
    public LoadingView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}