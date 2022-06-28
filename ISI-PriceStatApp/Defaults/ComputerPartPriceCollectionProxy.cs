using System.Collections.ObjectModel;
using Domain;
using LiveChartsCore.Defaults;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ISI_PriceStatApp.Defaults;

public class ComputerPartPriceCollectionProxy : ReactiveObject
{
    public ComputerPartPriceCollectionProxy(ComputerPart part, ObservableCollection<DateTimePoint> priceCollection)
    {
        this.PriceCollection = priceCollection;
        this.ComputerPart = part;
    }

    public ComputerPart ComputerPart { get; }
    
    [Reactive]
    public ObservableCollection<DateTimePoint> PriceCollection { get; set; }
}