using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SoterDevice;
using SoterWalletMobile.Helpers;
using SoterWalletMobile.ViewModels;
using Xamarin.Forms;

namespace SoterWalletMobile.Data
{
    public static class Repository
    {
        public static async Task LoadCoinTableFromDeviceAsync(ISoterDevice device)
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

        public static  ObservableCollection<WalletViewModel> GetWalletViewModels()
        {
            ObservableCollection<WalletViewModel> walletViewModels = new ObservableCollection<WalletViewModel>();
            using (var db = new DatabaseContext())
            {
                foreach (var coin in db.Coins)
                {
                    walletViewModels.Add(new WalletViewModel { Name = coin.CoinName, Shortcut = coin.CoinShortcut, Icon = ImageSource.FromFile(coin.CoinShortcut), Balance = coin.BalanceString, BalanceFiat = "$ 0.00" });
                }
            }
            return walletViewModels;
        }
    }
}
