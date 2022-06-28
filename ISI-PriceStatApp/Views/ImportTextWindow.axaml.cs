using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ISI_PriceStatApp.Views
{
    public partial class ImportTextWindow : Window
    {
        public ImportTextWindow()
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
