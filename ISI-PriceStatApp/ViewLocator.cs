using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ISI_PriceStatApp.ViewModels;

namespace ISI_PriceStatApp;

public class ViewLocator : IDataTemplate
{
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        return type != null ? 
            (Control)Activator.CreateInstance(type)! :
            new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object data) => data is ViewModelBase;
}