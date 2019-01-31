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
            SelectCoin((WalletViewModel)e.SelectedItem);
        }
    }
}
