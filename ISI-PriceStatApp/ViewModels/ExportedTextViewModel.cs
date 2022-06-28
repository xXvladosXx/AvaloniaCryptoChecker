using System.IO;
using System.Linq;
using System.Reactive;

using Avalonia;
using Avalonia.Controls;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ISI_PriceStatApp.ViewModels;

public class ExportedTextViewModel : ViewModelBase
{
    [Reactive]
    public string Text { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> CopyToClipboardCommand { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> SaveAsCommand { get; set; }

    public ExportedTextViewModel() : this(null!, "Example text!") { }

    public ExportedTextViewModel(Window view, string text)
    {
        this.Text = text;
        
        this.CopyToClipboardCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Application.Current!.Clipboard!.SetTextAsync(text);
        });

        this.SaveAsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var fileDialog = new OpenFileDialog()
            {
                AllowMultiple = false,
            };

            var path = (await fileDialog.ShowAsync(view))?.FirstOrDefault();

            if(path is not null)
            {
                await File.WriteAllTextAsync(path, text);
            }
        });
    }
}
