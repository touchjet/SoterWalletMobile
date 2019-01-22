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
        void ButtonStarPairing_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new PairingPage();
        }
    }
}
