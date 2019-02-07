using System.Collections.ObjectModel;
using SoterWalletMobile.Data;
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

            labelDeviceName.Text = string.Format("{0} ({1})", Repository.CurrentDevice.Name, Repository.CurrentDevice.Label);

            walletViewModels = Repository.GetWalletViewModels();
            summaryListView.ItemsSource = walletViewModels;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Repository.UpdateBalance();
        }

        async void DeviceNameLabel_Tapped(object sender, System.EventArgs e)
        {
            await DeviceCommPage.UpdateCoinTable(this);
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}
