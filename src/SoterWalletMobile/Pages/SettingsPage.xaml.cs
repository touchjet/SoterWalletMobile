using System;
using SoterDevice.Ble;
using SoterWalletMobile.Data;
using SoterWalletMobile.Helpers;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        async void WipeButton_Clicked(object sender, System.EventArgs e)
        {
            await SoterDeviceFactoryBle.Instance.ConnectByIdAsync(Settings.DeviceId);
            await SoterDeviceFactoryBle.Instance.CurrentDevice.WipeDeviceAsync();
            Settings.DeviceName = String.Empty;
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureDeleted();
            }
            Application.Current.MainPage = new StartPairingPage();
        }

        void ForgetButton_Clicked(object sender, System.EventArgs e)
        {
            Settings.DeviceName = String.Empty;
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureDeleted();
            }
            Application.Current.MainPage = new StartPairingPage();
        }
    }
}
