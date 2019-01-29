using System.Threading;
using System.Threading.Tasks;
using SoterDevice.Ble;
using SoterWalletMobile.Data;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class ConnectDevicePage : ContentPage
    {
        static bool connected;

        public static async Task<bool> Connect(Page parentPage)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            var connectPage = new ConnectDevicePage();
            connectPage.Disappearing += (sender, e) => { waitHandle.Set(); };
            await parentPage.Navigation.PushModalAsync(connectPage);
            await Task.Run(() => waitHandle.WaitOne());
            return connected;
        }

        public ConnectDevicePage()
        {
            InitializeComponent();
            connected = false;
            labelDeviceName.Text = string.Format("{0} ({1})", Repository.CurrentDevice.Name, Repository.CurrentDevice.Label);
        }

        async void ConnectButton_Clicked(object sender, System.EventArgs e)
        {
            messageLabel.Text = AppResources.Connecting;
            connected = await SoterDeviceFactoryBle.Instance.ConnectByIdAsync(Repository.CurrentDevice.BleGuid);
            if (!connected)
            {
                await DisplayAlert(AppResources.Error, AppResources.UnableToConnect, AppResources.OK);
                messageLabel.Text = AppResources.TurnOnWalletMessage;
                return;
            }
            await Navigation.PopModalAsync();
        }
    }
}
