using System;
using BlockchainService.Abstractions;
using BlockchainService.BlockCypherProxy.Client;

namespace SoterWalletMobile.Services
{
    public static class BitcoinService
    {
        readonly static IBitcoinServiceFactory bitcoinServiceFactory = new BitcoinServiceFactory("https://proxy1.digbig.io");
        public static IBitcoinService GetBitcoinService(string shortcut)
        {
            switch (shortcut)
            {
                case "TEST":
                    return bitcoinServiceFactory.GetService(CoinTypes.Bitcoin, true);
                case "BTC":
                    return bitcoinServiceFactory.GetService(CoinTypes.Bitcoin, false);
                case "LTC":
                    return bitcoinServiceFactory.GetService(CoinTypes.Litecoin, false);
                case "DOGE":
                    return bitcoinServiceFactory.GetService(CoinTypes.Dogecoin, false);
            }
            throw new Exception($"Coin {shortcut} not supported!");
        }
    }
}
