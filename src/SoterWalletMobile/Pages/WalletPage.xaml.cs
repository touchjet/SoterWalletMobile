using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SoterDevice.Ble;
using SoterWalletMobile.Data;
using SoterWalletMobile.Helpers;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class WalletPage : ContentPage
    {
        ObservableCollection<TokenViewModel> tokens = new ObservableCollection<TokenViewModel>();

        public WalletPage()
        {
            InitializeComponent();

            labelDeviceName.Text = Settings.DeviceName;

            using (var db = new DatabaseContext())
            {
                db.Database.EnsureCreated();
                foreach (var coin in db.Coins)
                {
                    tokens.Add(new TokenViewModel { Name = coin.CoinName, Shortcut = coin.CoinShortcut, Icon = ImageSource.FromFile(coin.CoinShortcut), Balance = coin.BalanceString, BalanceFiat = "$ 0.00" });
                }
            }
            summaryListView.ItemsSource = tokens;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new PinPage());
        }
    }

    public class TokenViewModel
    {
        public string Name { get; set; }
        public string Shortcut { get; set; }
        public string Balance { get; set; }
        public string BalanceFiat { get; set; }
        public ImageSource Icon { get; set; }
    }
}
