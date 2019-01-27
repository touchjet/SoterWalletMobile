using System;
using System.Threading.Tasks;
using SoterDevice.Ble;
using SoterWalletMobile.Data;
using SoterWalletMobile.Helpers;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class DeviceLabelPage : ContentPage
    {
        bool waitingForPin = false;

        public DeviceLabelPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
            deviceLabelEntry.Text = "";
            waitingForPin = false;
        }

        async Task<string> SoterDeviceEnterPinCallback()
        {
            await Navigation.PushAsync(new PinPage());
            waitingForPin = true;
            while( waitingForPin)
            {
                await Task.Delay(100);
            }
            return PinPage.PIN;
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            var device = SoterDeviceFactoryBle.Instance.CurrentDevice;
            device.EnterPinCallback += SoterDeviceEnterPinCallback; 
            await SoterDeviceFactoryBle.Instance.CurrentDevice.ResetDeviceAsync(deviceLabelEntry.Text);
            device.EnterPinCallback -= SoterDeviceEnterPinCallback;
            await Repository.LoadCoinTableFromDeviceAsync(device);
            device.Disconnect();
            Settings.DeviceId = device.Id;
            Settings.DeviceName = device.Name;
            Application.Current.MainPage = new NavigationPage(new MainTabbedPage());
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(deviceLabelEntry.Text))
            {
                okButton.IsEnabled = false;
                okButton.Style = (Style)Application.Current.Resources["disabledButtonStyle"];
            }
            else
            {
                okButton.IsEnabled = true;
                okButton.Style = (Style)Application.Current.Resources["normalButtonStyle"];
            }
        }
    }
}
