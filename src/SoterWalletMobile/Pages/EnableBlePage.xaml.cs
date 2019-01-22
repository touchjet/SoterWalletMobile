using System;
using System.Collections.Generic;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class EnableBlePage : ContentPage
    {
        public EnableBlePage()
        {
            InitializeComponent();
            CrossBluetoothLE.Current.StateChanged += Current_StateChanged;
        }

        void CheckState()
        {
            if (CrossBluetoothLE.Current.State == BluetoothState.On)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new StartPairingPage();
                });
            }
        }

        void Current_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            CheckState();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckState();
        }
    }
}
