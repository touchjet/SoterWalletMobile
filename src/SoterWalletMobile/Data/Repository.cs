using System;
using System.Linq;
using System.Threading.Tasks;
using SoterDevice;
using SoterWalletMobile.Helpers;

namespace SoterWalletMobile.Data
{
    public static class Repository
    {
        public static async Task LoadCoinTableFromDevice(ISoterDevice device)
        {
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureCreated();
                db.Transactions.Clear();
                db.Addresses.Clear();
                db.Coins.Clear();
                foreach (var coinType in await device.GetCoinTableAsync(72))
                {
                    if (Settings.SupportedCoins.Any(c => c.Equals(coinType.CoinShortcut)))
                    {
                        db.Coins.Add(new Models.Coin(coinType));
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
