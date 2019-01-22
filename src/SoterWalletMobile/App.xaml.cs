using System;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using SoterWalletMobile.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SoterWalletMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (CrossBluetoothLE.Current.State != BluetoothState.On)
            {
                MainPage = new EnableBlePage();
            }
            else
            {
                MainPage = new MainPage();
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
