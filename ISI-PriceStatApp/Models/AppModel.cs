using System.Collections.ObjectModel;
using System.Reactive;

using ISI_PriceStatApp.Defaults;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ISI_PriceStatApp.Models;

public class AppModel : ReactiveObject
{
    [Reactive]
    public ObservableCollection<CryptoCoinVisualProxy> CryptoCoinsVisuals { get; set; }
    
    [Reactive]
    public ObservableCollection<ComputerPartVisualProxy> ComputerPartsVisuals { get; set; }

    [Reactive]
    public ObservableCollection<CryptoCoinPriceCollectionProxy> CryptoCoinsPrices { get; set; }
    
    [Reactive]
    public ObservableCollection<ComputerPartPriceCollectionProxy> ComputerPartsPrices { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> ExportCryptoCoinsCommand { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> ExportComputerPartsCommand { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> ImportCryptoCoinsCommand { get; set; }

    [Reactive]
    public ReactiveCommand<Unit, Unit> ImportComputerPartsCommand { get; set; }

    public AppModel(ObservableCollection<CryptoCoinVisualProxy> cryptoCoinsVisuals, 
        ObservableCollection<ComputerPartVisualProxy> computerPartsVisuals, 
        ObservableCollection<CryptoCoinPriceCollectionProxy> cryptoCoinsPrices, 
        ObservableCollection<ComputerPartPriceCollectionProxy> computerPartsPrices,
        ReactiveCommand<Unit, Unit> exportCryptoCoinsCommand,
        ReactiveCommand<Unit, Unit> exportComputerPartsCommand,
        ReactiveCommand<Unit, Unit> importCryptoCoinsCommand,
        ReactiveCommand<Unit, Unit> importComputerPartsCommand)
    {
        CryptoCoinsVisuals = cryptoCoinsVisuals;
        ComputerPartsVisuals = computerPartsVisuals;
        CryptoCoinsPrices = cryptoCoinsPrices;
        ComputerPartsPrices = computerPartsPrices;
        ExportCryptoCoinsCommand = exportCryptoCoinsCommand;
        ExportComputerPartsCommand = exportComputerPartsCommand;
        ImportCryptoCoinsCommand = importCryptoCoinsCommand;
        ImportComputerPartsCommand = importComputerPartsCommand;
    }
}