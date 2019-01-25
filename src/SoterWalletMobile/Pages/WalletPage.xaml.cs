using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serilog;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class WalletPage : ContentPage
    {
        ObservableCollection<TokenViewModel> tokens = new ObservableCollection<TokenViewModel>();

        public WalletPage()
        {
            InitializeComponent();

            summaryListView.ItemsSource = tokens;

            tokens.Add(new TokenViewModel { Name = "Bitcoin", Shortcut = "BTC", Icon = ImageSource.FromFile("BTC"),Balance = "0.00 BTC", BalanceFiat = "$ 0.00" });
            tokens.Add(new TokenViewModel { Name = "Litecoin", Shortcut = "LTC", Icon = ImageSource.FromFile("LTC"), Balance = "0.00 LTC", BalanceFiat = "$ 0.00" });
            tokens.Add(new TokenViewModel { Name = "Ethereum", Shortcut = "ETH", Icon = ImageSource.FromFile("ETH"), Balance = "0.00 ETH", BalanceFiat = "$ 0.00" });
            tokens.Add(new TokenViewModel { Name = "Bitcoin Cash", Shortcut = "BCH", Icon = ImageSource.FromFile("BCH"), Balance = "0.00 BCH", BalanceFiat = "$ 0.00" });
            tokens.Add(new TokenViewModel { Name = "D Cash", Shortcut = "DASH", Icon = ImageSource.FromFile("DASH"), Balance = "0.00 DASH", BalanceFiat = "$ 0.00" });
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
