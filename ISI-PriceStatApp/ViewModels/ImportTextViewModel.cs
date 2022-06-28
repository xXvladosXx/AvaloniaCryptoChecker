using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

using Avalonia.Controls;

using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ISI_PriceStatApp.ViewModels;

public class ImportTextViewModel : ViewModelBase
{
    [Reactive]
    public string? Text { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> ReadFromFileCommand { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> ImportCommand { get; set; }

    public ImportTextViewModel() 
    {
        this.ReadFromFileCommand = null!;
        this.ImportCommand = null!;
    }

    public ImportTextViewModel(Window parentWindow, Window viewModelWindow) 
    {
        this.Text = null;

        this.ReadFromFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var fileDialog = new OpenFileDialog()
            {
                AllowMultiple = false,
            };

            var path = (await fileDialog.ShowAsync(parentWindow))?.FirstOrDefault();

            if (path is not null)
            {
                this.Text = await File.ReadAllTextAsync(path);
            }
        });

        this.ImportCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if(this.Text is null)
            {
                var mb = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams()
                {
                    ButtonDefinitions = MessageBox.Avalonia.Enums.ButtonEnum.Ok,
                    ContentTitle = "Error",
                    ContentMessage = "Nothing to import",
                    Icon = MessageBox.Avalonia.Enums.Icon.Error
                });

                _ = await mb.ShowDialog(parentWindow);
            }
            else
            {
                viewModelWindow.Close();
            }
        });
    }
}
