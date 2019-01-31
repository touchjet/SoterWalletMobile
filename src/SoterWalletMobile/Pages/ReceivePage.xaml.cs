using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoterWalletMobile.Data;
using SoterWalletMobile.ViewModels;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class ReceivePage : ContentPage
    {
        ObservableCollection<WalletViewModel> walletViewModels;

        public ReceivePage()
        {
            InitializeComponent();
            walletViewModels = Repository.GetWalletViewModels();
            coinListView.ItemsSource = walletViewModels;
            coinListView.SelectedItem = walletViewModels.FirstOrDefault();
        }

        void SelectCoin(WalletViewModel coin)
        {
            string address = coin.DefaultAddress;
            addressLabel.Text = address;
            ZXingBarcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            ZXingBarcode.BarcodeOptions.Width = 200;
            ZXingBarcode.BarcodeOptions.Height = 200;
            ZXingBarcode.BarcodeOptions.Margin = 5;
            ZXingBarcode.BarcodeValue = address;
            addressListView.ItemsSource = Repository.GetAddressViewModels(coin.Id);
        }

        void CoinList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SelectCoin((WalletViewModel)e.SelectedItem);
        }

        void AddressList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
