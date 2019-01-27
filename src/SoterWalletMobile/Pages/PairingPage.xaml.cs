using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Serilog;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SoterDevice;
using SoterDevice.Ble;
using SoterWalletMobile.Data;
using SoterWalletMobile.Helpers;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class PairingPage : ContentPage
    {
        public PairingPage()
        {
            InitializeComponent();
        }

        const int MAX_DRAW_STAGE = 3;
        object drawStageLock = new object();
        int drawStage;
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Device.StartTimer(TimeSpan.FromSeconds(0.3), () =>
            {
                canvasView.InvalidateSurface();
                lock (drawStageLock)
                {
                    if (drawStage < 0)
                    {
                        return false;
                    }
                    drawStage++;
                    if (drawStage > MAX_DRAW_STAGE)
                    {
                        drawStage = 0;
                    }
                }
                return true; // True = Repeat again, False = Stop the timer
            });

            if (Device.RuntimePlatform == Device.Android)
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need Location Permission", "Soter Wallet needs Location Permission in order to search for the device.", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Location Permission Denied", "Can not continue, try again.", "OK");
                }
            }

            ISoterDevice device = null;
            await SoterDeviceFactoryBle.Instance.StartDeviceSearchAsync();
            Log.Information("Wait for search results");
            await Task.Delay(500);
            Log.Information("Wait finished");
            await SoterDeviceFactoryBle.Instance.StopDeviceSearchAsync();
            Log.Information("Search stopped");
            if (SoterDeviceFactoryBle.Instance.Devices.Count == 0)
            {
                await DisplayAlert("Error", "Can't find any Soter Wallet device!", "OK");
            }
            else if (SoterDeviceFactoryBle.Instance.Devices.Count == 1)
            {
                device = SoterDeviceFactoryBle.Instance.Devices.First();
            }
            else
            {
                var action = await DisplayActionSheet("Select the device", "Cancel", null, SoterDeviceFactoryBle.Instance.Devices.Select(d => d.Name).ToArray());
                device = SoterDeviceFactoryBle.Instance.Devices.FirstOrDefault(d => d.Name.Equals(action));
            }
            if (device != null)
            {
                try
                {
                    await SoterDeviceFactoryBle.Instance.ConnectByIdAsync(device.Id);
                    await device.InitializeAsync();
                    lock (drawStageLock)
                    {
                        drawStage = -1;
                    }
                    if (device.Features.Initialized)
                    {
                        await Repository.LoadCoinTableFromDevice(device);
                        Settings.DeviceName = device.Name;
                        Settings.DeviceId = device.Id;
                        device.Disconnect();
                        Application.Current.MainPage = new NavigationPage(new MainTabbedPage());
                        return;
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new DeviceLabelPage());
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    await DisplayAlert("Error", "Can't connect to the Soter Wallet!", "OK");
                }
            }
            Application.Current.MainPage = new StartPairingPage();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            using (SKPaint paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 2;
                if (drawStage > 0)
                {
                    paint.Color = new SKColor(0x66416aff);
                    canvas.DrawCircle(info.Width / 2, info.Height / 2, info.Height / 10 * 3, paint);
                }
                if (drawStage > 1)
                {
                    paint.Color = new SKColor(0x33416aff);
                    canvas.DrawCircle(info.Width / 2, info.Height / 2, info.Height / 10 * 4, paint);
                }
                if (drawStage > 2)
                {
                    paint.Color = new SKColor(0x22416aff);
                    canvas.DrawCircle(info.Width / 2, info.Height / 2, info.Height / 10 * 5, paint);
                }

            }
        }
    }
}
