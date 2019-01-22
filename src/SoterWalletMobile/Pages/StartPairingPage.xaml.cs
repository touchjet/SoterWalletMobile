using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SoterDevice;
using SoterDevice.Ble;
using SoterWalletMobile.Helpers;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class StartPairingPage : ContentPage
    {
        public StartPairingPage()
        {
            InitializeComponent();
        }

        async void ButtonStarPairing_Clicked(object sender, System.EventArgs e)
        {
            ISoterDevice device = null;
            await SoterDeviceFactoryBle.Instance.StartDeviceSearchAsync();
            await Task.Delay(3000);
            await SoterDeviceFactoryBle.Instance.StopDeviceSearchAsync();
            if (SoterDeviceFactoryBle.Instance.Devices.Count == 0)
            {
                await DisplayAlert("Error", "Can't find any Soter Wallet device!", "OK");
            }
            else if (SoterDeviceFactoryBle.Instance.Devices.Count == 1)
            {
                device = SoterDeviceFactoryBle.Instance.Devices.FirstOrDefault();
            }
            else
            {
                var action = await DisplayActionSheet("Select the device", "Cancel", null, SoterDeviceFactoryBle.Instance.Devices.Select(d => d.Name).ToArray());
                device = SoterDeviceFactoryBle.Instance.Devices.FirstOrDefault(d => d.Name.Equals(action));
            }
            if (device != null)
            {
                try
                {
                    await device.InitializeAsync();
                    Settings.DeviceName = device.Name;
                    Application.Current.MainPage = new MainPage();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    await DisplayAlert("Error", "Can't connect to the Soter Wallet!", "OK");
                }
            }
        }
    }
}
