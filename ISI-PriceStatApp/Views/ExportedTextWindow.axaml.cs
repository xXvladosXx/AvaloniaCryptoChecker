using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ISI_PriceStatApp.Views
{
    public partial class ExportedTextWindow : Window
    {
        public ExportedTextWindow()
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
}
