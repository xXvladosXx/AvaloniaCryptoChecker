using System;

namespace Domain;

public enum CryptoCoin
{
    Bitcoin,
    Ethereum,
    Dogecoin
}

public enum ComputerPart
{
    VideoCard,
    Cpu,
    SsdDisk
}

public record Stat(DateTime Time, double Price);

public record CryptoCoinStats(CryptoCoin Coin, Stat[] Stats);

public record ComputerPartStats(ComputerPart Part, Stat[] Stats);