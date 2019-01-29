﻿using System.Collections.ObjectModel;
using SoterDevice.Ble;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        async void DeviceNameLabel_Tapped(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new DeviceCommPage(CommStage.UpdateCoinTable));
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}
