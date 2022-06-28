using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services;

public static class ComputerPartServices
{
    public static ComputerPart[] AllPossibleComputerParts { get; } =
        Enum.GetValues<ComputerPart>();

    public static async Task<ComputerPartStats[]> FetchAll()
    {
        return await Task.Run(async () =>
        {
            await Task.Delay(1);

            static Stat[] NextNRandomPricesWithDateTime(int n)
            {
                var r = new Random();
                var day = 0;

                return Enumerable.Range(0, n)
                    .Select(i => new Stat(DateTime.Now.AddDays(--day), r.Next(0, 5000) + r.NextDouble()))
                    .ToArray();
            }

            var r = new Random();

            return AllPossibleComputerParts
                .Select(p => new ComputerPartStats(p, NextNRandomPricesWithDateTime(20)))
                .ToArray();
        });
    }
}