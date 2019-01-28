using System;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class DeviceLabelPage : ContentPage
    {
        public DeviceLabelPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
            deviceLabelEntry.Text = "";
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new DeviceInitializationPage(deviceLabelEntry.Text));
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
