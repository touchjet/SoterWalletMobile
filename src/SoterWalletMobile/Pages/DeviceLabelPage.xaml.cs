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

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await DeviceCommPage.ResetDevice(this, deviceLabelEntry.Text);
            Application.Current.MainPage = new NavigationPage(new MainTabbedPage());
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
