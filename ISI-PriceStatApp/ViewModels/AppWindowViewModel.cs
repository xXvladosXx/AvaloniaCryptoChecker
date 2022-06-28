using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ISI_PriceStatApp.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using ReactiveUI.Fody.Helpers;

using Domain.Services;
using ISI_PriceStatApp.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using System.Linq;
using Domain;
using ReactiveUI;
using ISI_PriceStatApp.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nancy.Json;
using MessageBox.Avalonia;

namespace ISI_PriceStatApp.ViewModels;

public class AppWindowViewModel : ViewModelBase
{
    #region async load/execution logic with 'wait plesase' layer

    [Reactive] public bool IsContentLoading { get; set; } = false;

    private async Task PerformOperationAsync(Func<Task> asyncOperation)
    {
        var timeOutNoWaitLayer = Task.Delay(100);
        var operation = asyncOperation();

        this.IsContentLoading = await Task.WhenAny(timeOutNoWaitLayer, operation) == timeOutNoWaitLayer;

        await operation;
        this.IsContentLoading = false;
    }

    private async Task<T> LoadAsync<T>(Func<Task<T>> asyncLoadOperation)
    {
        var timeOutNoWaitLayer = Task.Delay(100);
        var operation = asyncLoadOperation();

        this.IsContentLoading = await Task.WhenAny(timeOutNoWaitLayer, operation) == timeOutNoWaitLayer;

        var result = await operation;
        this.IsContentLoading = false;

        return result;
    }

    #endregion

    #region visualisation bindable data

    // just date-time format for chart axe
    public ICartesianAxis[] XAxesCustom { get; }

    [Reactive]
    public ObservableCollection<ISeries> Series { get; set; }

    #endregion

    public Task<AppModel> AppModel { get; }

    public AppWindowViewModel()
    {
        this.XAxesCustom = new ICartesianAxis[] {
            new Axis {
                Labeler = val => new DateTime((long) val).ToString(""),
                LabelsRotation = 15,
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                MinStep = TimeSpan.FromDays(1).Ticks,
            }
        };

        this.Series = default!;
        this.AppModel = LoadAsync(async () => {
            var fetchedComp = await ComputerPartServices.FetchAll();
            var fetchedCrypto = await CryptoCoinServices.FetchAll();

            fetchedCrypto = (await ImportCryptoCoinsServices.LoadFromDbAsync())
                .Concat(fetchedCrypto)
                .GroupBy(x => x.Coin)
                .Select(x => new CryptoCoinStats(x.Key, x.SelectMany(y => y.Stats).Distinct().OrderBy(x => x.Time).ToArray()))
                .ToArray();

            fetchedComp = (await ImportComputerPartsServices.LoadFromDbAsync())
                .Concat(fetchedComp)
                .GroupBy(x => x.Part)
                .Select(x => new ComputerPartStats(x.Key, x.SelectMany(y => y.Stats).Distinct().OrderBy(x => x.Time).ToArray()))
                .ToArray();

            var compPrices = new ObservableCollection<ComputerPartPriceCollectionProxy>();
            var compVisuals = new ObservableCollection<ComputerPartVisualProxy>();

            var cryptoPrices = new ObservableCollection<CryptoCoinPriceCollectionProxy>();
            var cryptoVisuals = new ObservableCollection<CryptoCoinVisualProxy>();

            this.Series = new ObservableCollection<ISeries>();

            foreach (var (part, stats) in fetchedComp)
            {
                var priceHistory = new ObservableCollection<DateTimePoint>();

                foreach (var (dateTime, price) in stats)
                {
                    priceHistory.Add(new DateTimePoint(dateTime, price));
                }

                var series = new LineSeries<DateTimePoint>()
                {
                    Values = priceHistory,
                    Name = part.ToString(),
                };

                this.Series.Add(series);
                compPrices.Add(new ComputerPartPriceCollectionProxy(part, priceHistory));
                compVisuals.Add(new ComputerPartVisualProxy(series));
            }

            foreach (var (crypto, stats) in fetchedCrypto)
            {
                var priceHistory = new ObservableCollection<DateTimePoint>();
            
                foreach (var (dateTime, price) in stats)
                {
                    priceHistory.Add(new DateTimePoint(dateTime, price));
                }
            
                var series = new LineSeries<DateTimePoint>()
                {
                    Values = priceHistory,
                    Name = crypto.ToString(),
                };
            
                this.Series.Add(series);
                cryptoPrices.Add(new CryptoCoinPriceCollectionProxy(crypto, priceHistory));
                cryptoVisuals.Add(new CryptoCoinVisualProxy(series));
            }

            await ImportCryptoCoinsServices.ClearAllAndInsertAsync(fetchedCrypto);
            await ImportComputerPartsServices.ClearAllAndInsertAsync(fetchedComp);

            var exportCryptoCoinsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var mainWindow = (Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow;
                var statsToExport = cryptoPrices
                    .Select(x => new CryptoCoinStats(x.CryptoCoin, x.PriceCollection.OrderByDescending(y => y.DateTime).Select(x => new Stat(x.DateTime, x.Value!.Value)).ToArray()))
                    .ToArray();

                var textToExport = JsonConvert.SerializeObject(statsToExport, Formatting.Indented, new StringEnumConverter());

                var exportTextWindow = new ExportedTextWindow()
                {
                    DataContext = new ExportedTextViewModel(mainWindow, textToExport)
                };

                await exportTextWindow.ShowDialog(mainWindow);
            });

            var exportComputerPartsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var mainWindow = (Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow;
                var statsToExport = compPrices
                    .Select(x => new ComputerPartStats(x.ComputerPart, x.PriceCollection.OrderByDescending(y => y.DateTime).Select(x => new Stat(x.DateTime, x.Value!.Value)).ToArray()))
                    .ToArray();

                var textToExport = JsonConvert.SerializeObject(statsToExport, Formatting.Indented, new StringEnumConverter());

                var exportTextWindow = new ExportedTextWindow()
                {
                    DataContext = new ExportedTextViewModel(mainWindow, textToExport)
                };

                await exportTextWindow.ShowDialog(mainWindow);
            });

            var importCryptoCoinsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var mainWindow = (Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow;

                var exportTextWindow = new ImportTextWindow();
                var dataContext = new ImportTextViewModel(mainWindow, exportTextWindow);
                exportTextWindow.DataContext = dataContext;

                await exportTextWindow.ShowDialog(mainWindow);

                var text = dataContext.Text;

                if(text == null)
                {
                    return;
                }

                try
                {
                    var stats = new JavaScriptSerializer().Deserialize<CryptoCoinStats[]>(text);

                    fetchedCrypto = fetchedCrypto
                        .Concat(stats)
                        .GroupBy(x => x.Coin)
                        .Select(x => new CryptoCoinStats(x.Key, x.SelectMany(y => y.Stats).Distinct().OrderBy(x => x.Time).ToArray()))
                        .ToArray();

                    await ImportComputerPartsServices.ClearAllAndInsertAsync(fetchedComp);
                }
                catch (Exception ex)
                {
                    var mb = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBox.Avalonia.DTO.MessageBoxStandardParams()
                    {
                        ContentHeader = "Error",
                        ContentMessage = ex.Message,
                        Icon = MessageBox.Avalonia.Enums.Icon.Error,
                    });

                    await mb.ShowDialog(mainWindow);
                }
            });

            var importComputerPartsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var mainWindow = (Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow;

                var exportTextWindow = new ImportTextWindow();
                var dataContext = new ImportTextViewModel(mainWindow, exportTextWindow);
                exportTextWindow.DataContext = dataContext;

                await exportTextWindow.ShowDialog(mainWindow);

                var text = dataContext.Text;

                try
                {
                    var stats = new JavaScriptSerializer().Deserialize<ComputerPartStats[]>(text);

                    fetchedComp = fetchedComp
                        .Concat(stats)
                        .GroupBy(x => x.Part)
                        .Select(x => new ComputerPartStats(x.Key, x.SelectMany(y => y.Stats).Distinct().OrderBy(x => x.Time).ToArray()))
                        .ToArray();

                    await ImportCryptoCoinsServices.ClearAllAndInsertAsync(fetchedCrypto);
                }
                catch (Exception ex)
                {
                    var mb = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBox.Avalonia.DTO.MessageBoxStandardParams()
                    {
                        ContentHeader = "Error",
                        ContentMessage = ex.Message,
                        Icon = MessageBox.Avalonia.Enums.Icon.Error,
                    });

                    await mb.ShowDialog(mainWindow);
                }
            });

            return new AppModel(
                cryptoVisuals, 
                compVisuals, 
                cryptoPrices, 
                compPrices,
                exportCryptoCoinsCommand,
                exportComputerPartsCommand,
                importCryptoCoinsCommand,
                importComputerPartsCommand
            );
        });
    }

    
}