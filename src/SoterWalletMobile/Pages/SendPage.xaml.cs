using System;
using System.Collections.ObjectModel;
using System.Linq;
using SoterWalletMobile.Data;
using SoterWalletMobile.ViewModels;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class SendPage : ContentPage
    {
        ObservableCollection<WalletViewModel> walletViewModels;

        public SendPage()
        {
            InitializeComponent();
            walletViewModels = Repository.GetWalletViewModels();
            coinListView.ItemsSource = walletViewModels;
            coinListView.SelectedItem = walletViewModels.FirstOrDefault();
        }

        void SelectCoin(WalletViewModel selected)
        {
            labelBalance.Text = selected.Balance;
            labelCurrency.Text = selected.Shortcut;
            amountEntry.Text = string.Empty;
            toAddressEntry.Text = string.Empty;
        }

        async void ScanButton_Tapped(object sender, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                toAddressEntry.Text = result.Text;
            }
        }

        void NextButton_Clicked(object sender, EventArgs e)
        {
            DisplayAlert(AppResources.Error, "Not implemented yet!", "WELL DONE");
        }

        void CoinList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SelectCoin((WalletViewModel)e.SelectedItem);
        }
    }
}
