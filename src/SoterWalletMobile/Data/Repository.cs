using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BlockchainService.Abstractions;
using BlockchainService.BlockCypherProxy.Client;
using Microsoft.EntityFrameworkCore;
using SoterDevice;
using SoterDevice.Models;
using SoterWalletMobile.Helpers;
using SoterWalletMobile.Models;
using SoterWalletMobile.ViewModels;
using Touchjet.BinaryUtils;
using Xamarin.Forms;

namespace SoterWalletMobile.Data
{
    public static class Repository
    {
        public static readonly string[] SupportedCoins = { "BTC", "TEST", "DOGE", "LTC" };

        static WalletDevice _currentDevice;
        public static WalletDevice CurrentDevice
        {
            get
            {
                if (_currentDevice == null)
                {
                    using (var db = new DatabaseContext())
                    {
                        db.Database.EnsureCreated();
                        _currentDevice = db.WalletDevices.SingleOrDefault();
                        if (_currentDevice == null)
                        {
                            db.WalletDevices.Clear();
                            _currentDevice = new WalletDevice();
                            db.WalletDevices.Add(_currentDevice);
                            db.SaveChanges();
                        }
                    }
                }
                return _currentDevice;
            }
        }

        public static void SaveCurrentDeviceToDb(ISoterDevice device)
        {
            _currentDevice = new WalletDevice
            {
                Name = device.Name,
                BleGuid = device.Id,
                Label = device.Features.Label,
                Uuid = device.Features.DeviceId,
                Initialized = device.Features.Initialized,
                Model = device.Features.Model,
                BootloaderHash = device.Features.BootloaderHash.ToHex(),
                FirmwareHash = device.Features.FirmwareHash.ToHex(),
                Language = device.Features.Language,
                MajorVersion = device.Features.MajorVersion,
                MinorVersion = device.Features.MinorVersion,
                PatchVersion = device.Features.PatchVersion,

            };

            using (var db = new DatabaseContext())
            {
                db.WalletDevices.Clear();
                db.WalletDevices.Add(_currentDevice);
                db.SaveChanges();
            }
        }

        public static async Task LoadCoinTableFromDeviceAsync(ISoterDevice device)
        {
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureCreated();
                db.Transactions.Clear();
                db.Addresses.Clear();
                db.Coins.Clear();
                var coinTable = await device.GetCoinTableAsync(48);
                device.CoinUtility = new CoinUtility(coinTable);
                foreach (var coinType in coinTable)
                {
                    if (SupportedCoins.Any(c => c.Equals(coinType.CoinShortcut)))
                    {
                        db.Coins.Add(new Models.Coin(coinType));
                    }
                }
                db.SaveChanges();
                foreach (var coin in db.Coins)
                {
                    var addressPath = new BIP44AddressPath(coin.Segwit, AddressUtilities.UnhardenNumber(coin.Bip44AccountPath), 0, false, 0);
                    var addressStr = await device.GetAddressAsync((IAddressPath)addressPath, false, false);
                    var address = new Address()
                    {
                        CoinId = coin.Id,
                        Account = 0,
                        Change = 0,
                        AddressIndex = 0,
                        AddressString = addressStr,
                        CoinType = addressPath.CoinType,
                        Purpose = addressPath.Purpose,
                    };
                    db.Addresses.Add(address);
                }
                db.SaveChanges();
            }
        }

        readonly static IBitcoinServiceFactory bitcoinServiceFactory = new BitcoinServiceFactory("https://proxy1.digbig.io");
        public static IBitcoinService GetBitcoinService(Coin coin)
        {
            switch (coin.CoinShortcut)
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
            throw new System.Exception($"Coin {coin.CoinName} not supported!");
        }

        public static async Task UpdateBalance()
        {
            IBitcoinService bitcoinService;
            using (var db = new DatabaseContext())
            {
                await db.SaveChangesAsync();

                foreach (var address in db.Addresses.Include(a=>a.Coin))
                {
                    bitcoinService = GetBitcoinService(address.Coin);
                    var bal = await bitcoinService.GetBalanceAsync(address.AddressString);
                    address.ConfirmedBalance = (ulong)bal.Balance;
                    address.UnconfirmedBalance = (ulong)bal.UnconfirmedBalance;
                }
                await db.SaveChangesAsync();
                foreach (var coin in db.Coins.Include(c=>c.Addresses))
                {
                    var viewModel = walletViewModels.SingleOrDefault(v => v.Shortcut.Equals(coin.CoinShortcut));
                    if (viewModel != null)
                    {
                        viewModel.Balance = coin.BalanceString;
                    }
                }
            }

        }

        static ObservableCollection<WalletViewModel> walletViewModels = new ObservableCollection<WalletViewModel>();
        public static ObservableCollection<WalletViewModel> GetWalletViewModels()
        {
            using (var db = new DatabaseContext())
            {
                foreach (var coin in db.Coins.Include(c=>c.Addresses))
                {
                    if (!walletViewModels.Any(v => v.Shortcut.Equals(coin.CoinShortcut)))
                    {
                        walletViewModels.Add(new WalletViewModel { Name = coin.CoinName, Shortcut = coin.CoinShortcut, Icon = GetIconImageSource(coin.CoinShortcut), Balance = coin.BalanceString, BalanceFiat = "$ 0.00" });
                    }
                }
            }
            return walletViewModels;
        }

        static ImageSource GetIconImageSource(string coinShortcut)
        {
            return ImageSource.FromFile(coinShortcut.Equals("TEST") ? "BTC" : coinShortcut);
        }
    }
}
