using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BlockchainService.Abstractions;
using BlockchainService.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using SoterDevice;
using SoterDevice.Models;
using SoterWalletMobile.Helpers;
using SoterWalletMobile.Models;
using SoterWalletMobile.Services;
using SoterWalletMobile.ViewModels;
using Touchjet.BinaryUtils;
using Xamarin.Forms;

namespace SoterWalletMobile.Data
{
    public static class Repository
    {
        public static readonly string[] SupportedCoins = { "BTC", "TEST", "DOGE", "LTC" };

        public static List<WalletDevice> _walletDevices;

        public static void LoadWalletDevices()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    db.Database.EnsureCreated();
                    db.WalletDevices.RemoveRange(db.WalletDevices.Where(d => string.IsNullOrWhiteSpace(d.Name)));
                    db.SaveChanges();
                    _walletDevices = db.WalletDevices.ToList();
                    _currentDevice = _walletDevices.FirstOrDefault();
                    if (_currentDevice == null)
                    {
                        db.WalletDevices.Add(new WalletDevice());
                        db.SaveChanges();
                        _currentDevice = db.WalletDevices.First();
                        _walletDevices.Add(_currentDevice);
                    }
                }
            }
            catch (Exception ex)
            {
                _currentDevice = new WalletDevice();
                Log.Error(ex.ToString());
            }
        }

        static WalletDevice _currentDevice;
        public static WalletDevice CurrentDevice
        {
            get
            {
                if ((_currentDevice == null) || (String.IsNullOrWhiteSpace(_currentDevice.Name)))
                {
                    LoadWalletDevices();
                }
                return _currentDevice;
            }
            set
            {
                _currentDevice = value;
            }
        }

        public static void SaveCurrentDeviceToDb(ISoterDevice device)
        {
            if (_currentDevice == null)
            {
                LoadWalletDevices();
            }
            using (var db = new DatabaseContext())
            {
                if (!string.IsNullOrWhiteSpace(_currentDevice.Name) && _currentDevice.Name.Equals(device.Name))
                {
                    _currentDevice.BleGuid = device.Id;
                    _currentDevice.Label = device.Features.Label;
                    _currentDevice.Uuid = device.Features.DeviceId;
                    _currentDevice.Initialized = device.Features.Initialized;
                    _currentDevice.Model = device.Features.Model;
                    _currentDevice.BootloaderHash = device.Features.BootloaderHash.ToHex();
                    _currentDevice.FirmwareHash = device.Features.FirmwareHash.ToHex();
                    _currentDevice.Language = device.Features.Language;
                    _currentDevice.MajorVersion = device.Features.MajorVersion;
                    _currentDevice.MinorVersion = device.Features.MinorVersion;
                    _currentDevice.PatchVersion = device.Features.PatchVersion;

                    db.WalletDevices.Update(_currentDevice);
                    db.SaveChanges();
                }
                else
                {
                    db.WalletDevices.Add(new WalletDevice
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
                    });
                    db.SaveChanges();
                    _currentDevice = db.WalletDevices.First(w => w.Name.Equals(device.Name));
                }
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
                        db.Coins.Add(new Coin(coinType) { WalletDeviceId = _currentDevice.Id });
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

        public static async Task UpdateBalance()
        {
            IBitcoinService bitcoinService;
            using (var db = new DatabaseContext())
            {
                await db.SaveChangesAsync();

                foreach (var address in db.Addresses.Include(a => a.Coin))
                {
                    bitcoinService = BitcoinService.GetBitcoinService(address.Coin.CoinShortcut);
                    var bal = await bitcoinService.GetBalanceAsync(address.AddressString);
                    address.ConfirmedBalance = bal.Balance;
                    address.UnconfirmedBalance = bal.UnconfirmedBalance;
                    db.Addresses.Update(address);
                }
                await db.SaveChangesAsync();
                foreach (var coin in db.Coins.Include(c => c.Addresses))
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
            try
            {
                using (var db = new DatabaseContext())
                {
                    foreach (var coin in db.Coins.Include(c => c.Addresses))
                    {
                        if (!walletViewModels.Any(v => v.Shortcut.Equals(coin.CoinShortcut)))
                        {
                            walletViewModels.Add(new WalletViewModel { Id = coin.Id, Name = coin.CoinName, Shortcut = coin.CoinShortcut, Icon = GetIconImageSource(coin.CoinShortcut), Balance = coin.BalanceString, BalanceFiat = "$ 0.00", DefaultAddress = coin.Addresses.FirstOrDefault().AddressString });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return walletViewModels;
        }

        static ImageSource GetIconImageSource(string coinShortcut)
        {
            return ImageSource.FromFile(coinShortcut.Equals("TEST") ? "BTC" : coinShortcut);
        }

        public static List<AddressViewModel> GetAddressViewModels(int coinId)
        {
            var addresses = new List<AddressViewModel>();
            try
            {
                using (var db = new DatabaseContext())
                {
                    foreach (var address in db.Addresses.Where(a => a.CoinId == coinId))
                    {
                        addresses.Add(new AddressViewModel() { Id = address.Id, CoinId = address.CoinId, AddressString = address.AddressString });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return addresses;
        }
    }
}
