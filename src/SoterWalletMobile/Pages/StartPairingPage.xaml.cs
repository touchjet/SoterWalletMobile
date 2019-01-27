using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class StartPairingPage : ContentPage
    {
        public StartPairingPage()
        {
            InitializeComponent();
        }

        void ButtonStarPairing_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new PairingPage();
        }
    }
}
