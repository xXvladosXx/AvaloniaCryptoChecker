using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;

namespace Domain.Services;

public static class CryptoCoinServices
{
    public static CryptoCoin[] AllPossibleCryptoCoins { get; } =
        Enum.GetValues<CryptoCoin>();

    private static string API_KEY = "66d1629a-fa54-414c-b4da-c694fa19b3d3";

    private static int GetId(CryptoCoin crypto) => crypto switch
    {
        CryptoCoin.Bitcoin => 1,
        CryptoCoin.Ethereum => 1027,
        CryptoCoin.Dogecoin => 12,
        _ => throw new Exception()
    };

    public static async Task<CryptoCoinStats[]> FetchAll()
    {
        return await Task.Run(() =>
        {
            var r = new Random();
            var cryptoIDs = AllPossibleCryptoCoins.Select(GetId).ToArray();

            static dynamic GetCrypto(string cryptoID)
            {
                var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest");

                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["id"] = cryptoID;
                URL.Query = queryString.ToString();

#pragma warning disable SYSLIB0014 // Type or member is obsolete
                var client = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
                client.Headers.Add("Accepts", "application/json");

                dynamic dataCrypto = JsonConvert.DeserializeObject<dynamic>(client.DownloadString(URL.ToString()))!;
                dynamic idCrypto = dataCrypto.data;
                return idCrypto[cryptoID]["quote"]["USD"];
            }

            static Stat[] ParseData(dynamic crypto)
            {
                string curPrice = crypto.price;
                string oneHourPer = crypto.percent_change_1h;
                string oneDayPer = crypto.percent_change_24h;
                string oneWeekPer = crypto.percent_change_7d;
                string oneMonthPer = crypto.percent_change_30d;

                double curPriceValue = double.Parse(curPrice, CultureInfo.InvariantCulture.NumberFormat);

                double oneHourPerF = double.Parse(oneHourPer, CultureInfo.InvariantCulture.NumberFormat);
                double oneHourVal = oneHourPerF / 100 * curPriceValue;
                double finalOneHour = curPriceValue + oneHourVal;

                double oneDayHour = double.Parse(oneDayPer, CultureInfo.InvariantCulture.NumberFormat);
                double oneDayVal = oneDayHour / 100 * curPriceValue;
                double finalOneDay = curPriceValue + oneDayVal;

                double oneWeekPerF = double.Parse(oneWeekPer, CultureInfo.InvariantCulture.NumberFormat);
                double oneWeekVal = oneWeekPerF / 100 * curPriceValue;
                double finalOneWeek = curPriceValue + oneWeekVal;

                double oneMonthPerF = double.Parse(oneMonthPer, CultureInfo.InvariantCulture.NumberFormat);
                double oneMonthVal = oneMonthPerF / 100 * curPriceValue;
                double finalOneMonth = curPriceValue + oneMonthVal;

                DateTime localDate = DateTime.Now;
                DateTime dateHour = DateTime.Now.AddHours(-1);
                DateTime dateDay = DateTime.Now.AddDays(-1);
                DateTime dateWeek = DateTime.Now.AddDays(-7);
                DateTime dateMonth = DateTime.Now.AddDays(-30);

                return new[]
                {
                    new Stat(localDate, curPriceValue),
                    new Stat(dateHour, finalOneHour),
                    new Stat(dateDay, finalOneDay),
                    new Stat(dateWeek, finalOneWeek),
                    new Stat(dateMonth, finalOneMonth),
                };
            }

            return cryptoIDs
                .Select(x => x.ToString())
                .Select(GetCrypto)
                .Select(ParseData)
                .Zip(AllPossibleCryptoCoins)
                .Select(x => new CryptoCoinStats(x.Second, x.First))
                .ToArray();
        });
    }
}