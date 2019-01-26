using System;
using System.Collections.Generic;
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

        void Handle_Clicked(object sender, System.EventArgs e)
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
