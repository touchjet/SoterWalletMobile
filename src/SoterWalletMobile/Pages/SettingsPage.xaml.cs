using SoterDevice.Ble;
using SoterWalletMobile.Data;
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
            await SoterDeviceFactoryBle.Instance.ConnectByIdAsync(Repository.CurrentDevice.BleGuid);
            await SoterDeviceFactoryBle.Instance.CurrentDevice.WipeDeviceAsync();
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureDeleted();
            }
            Application.Current.MainPage = new StartPairingPage();
        }

        void ForgetButton_Clicked(object sender, System.EventArgs e)
        {
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureDeleted();
            }
            Application.Current.MainPage = new StartPairingPage();
        }
    }
}
