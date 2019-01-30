using System;
using Serilog;
using SoterDevice.Ble;
using SoterWalletMobile.Data;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class DeviceCommPage : ContentPage
    {
        bool reEntry;
        string label;
        bool connected;
        CommStage stage;

        public DeviceCommPage()
        {
            InitializeComponent();
        }

        public DeviceCommPage(CommStage stage, string label = "", bool connected = false)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.stage = stage;
            this.label = label;
            this.connected = connected;
            reEntry = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!reEntry)
            {
                reEntry = true;
                if ((!connected) && (!await ConnectDevicePage.Connect(this)))
                {
                    await Navigation.PopModalAsync();
                }
                try
                {
                    var device = SoterDeviceFactoryBle.Instance.CurrentDevice;
                    deviceLabel.Text = device.Name;
                    PinPage.ParentPage = this;
                    device.DeviceButtonRequestCallback += Device_DeviceButtonRequestCallback;

                    if (stage == CommStage.WipeDevice)
                    {
                        messageLabel.Text = AppResources.ConfirmWipeMessage;
                        await SoterDeviceFactoryBle.Instance.CurrentDevice.WipeDeviceAsync();
                        stage = CommStage.ResetDevice;
                    }

                    if (stage == CommStage.ResetDevice)
                    {
                        messageLabel.Text = AppResources.InitializeDeviceMessage;
                        await SoterDeviceFactoryBle.Instance.CurrentDevice.ResetDeviceAsync(label);
                        stage = CommStage.UpdateCoinTable;
                    }

                    if (stage == CommStage.UpdateCoinTable)
                    {
                        messageLabel.Text = AppResources.LoadingMessage;
                        await device.InitializeAsync();
                        Repository.SaveCurrentDeviceToDb(device);
                        await Repository.LoadCoinTableFromDeviceAsync(device);
                        stage = CommStage.Done;
                    }

                    PinPage.ParentPage = null;
                    device.DeviceButtonRequestCallback -= Device_DeviceButtonRequestCallback;
                    device.Disconnect();
                    Application.Current.MainPage = new NavigationPage(new MainTabbedPage());
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    await Navigation.PopModalAsync();
                }
            }
        }

        void Device_DeviceButtonRequestCallback()
        {
            switch (stage)
            {
                case CommStage.ResetDevice:
                    messageLabel.Text = AppResources.RecoverSentenceMessage;
                    break;
            }
        }
    }

    public enum CommStage
    {
        WipeDevice,
        ResetDevice,
        UpdateCoinTable,
        Done
    }
}
