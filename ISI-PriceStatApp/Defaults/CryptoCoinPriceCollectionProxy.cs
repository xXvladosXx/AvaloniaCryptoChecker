using System.Collections.ObjectModel;
using Domain;
using LiveChartsCore.Defaults;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ISI_PriceStatApp.Defaults;

public class CryptoCoinPriceCollectionProxy : ReactiveObject
{
    public CryptoCoinPriceCollectionProxy(CryptoCoin coin, ObservableCollection<DateTimePoint> priceCollection)
    {
        this.PriceCollection = priceCollection;
        this.CryptoCoin = coin;
    }

    public CryptoCoin CryptoCoin { get; }
    
    [Reactive]
    public ObservableCollection<DateTimePoint> PriceCollection { get; set; }
}