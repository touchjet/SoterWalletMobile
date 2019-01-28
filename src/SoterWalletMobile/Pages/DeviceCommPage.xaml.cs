using SoterDevice.Ble;
using SoterWalletMobile.Data;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class DeviceCommPage : ContentPage
    {
        bool reEntry;
        string label;
        CommStage stage;

        public DeviceCommPage()
        {
            InitializeComponent();
        }

        public DeviceCommPage(CommStage stage, string label = "")
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.stage = stage;
            this.label = label;
            reEntry = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!reEntry)
            {
                reEntry = true;
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
