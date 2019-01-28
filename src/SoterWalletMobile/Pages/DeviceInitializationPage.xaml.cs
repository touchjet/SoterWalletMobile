using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoterDevice.Ble;
using SoterDevice.Contracts;
using SoterWalletMobile.Data;
using SoterWalletMobile.Helpers;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class DeviceInitializationPage : ContentPage
    {
        string deviceLabel;
        bool beginInitialize;

        public DeviceInitializationPage()
        {
            InitializeComponent();
            this.deviceLabel = String.Empty;
        }

        public DeviceInitializationPage(string deviceLabel)
        {
            InitializeComponent();
            this.deviceLabel = deviceLabel;
            beginInitialize = true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (beginInitialize)
            {
                beginInitialize = false;
                var device = SoterDeviceFactoryBle.Instance.CurrentDevice;
                device.EnterPinCallback += Device_EnterPinCallback; ;
                device.DeviceButtonRequestCallback += Device_DeviceButtonRequestCallback;
                await SoterDeviceFactoryBle.Instance.CurrentDevice.ResetDeviceAsync(deviceLabel);
                device.EnterPinCallback -= Device_EnterPinCallback;
                device.DeviceButtonRequestCallback -= Device_DeviceButtonRequestCallback;
                await Repository.LoadCoinTableFromDeviceAsync(device);
                device.Disconnect();
                Settings.DeviceId = device.Id;
                Settings.DeviceName = device.Name;
                Application.Current.MainPage = new NavigationPage(new MainTabbedPage());
            }
        }

        async Task<String> Device_EnterPinCallback(PinMatrixRequestType pinType)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            var pinPage = new PinPage(pinType);
            pinPage.Disappearing += (sender, e) => { waitHandle.Set(); };
            await Navigation.PushAsync(pinPage);
            await Task.Run(() => waitHandle.WaitOne());
            return PinPage.PIN;
        }


        void Device_DeviceButtonRequestCallback()
        {
            messageLabel.Text = AppResources.RecoverSentenceMessage;
        }

    }
}
