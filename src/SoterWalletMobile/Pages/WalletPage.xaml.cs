using System.Collections.ObjectModel;
using SoterWalletMobile.Data;
using SoterWalletMobile.Helpers;
using SoterWalletMobile.ViewModels;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class WalletPage : ContentPage
    {
        ObservableCollection<WalletViewModel> walletViewModels;

        public WalletPage()
        {
            InitializeComponent();

            labelDeviceName.Text = Settings.DeviceName;

            walletViewModels = Repository.GetWalletViewModels();
            summaryListView.ItemsSource = walletViewModels;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {

        }
    }
}
