using System;
using System.Collections.ObjectModel;
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
        }

        void ScanButton_Tapped(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }

        void NextButton_Clicked(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }

        void CoinList_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
