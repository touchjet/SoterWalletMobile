using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            ISoterDevice device = null;
            await SoterDeviceFactoryBle.Instance.StartDeviceSearchAsync();
            await Task.Delay(500);
            await SoterDeviceFactoryBle.Instance.StopDeviceSearchAsync();
            if (SoterDeviceFactoryBle.Instance.Devices.Count == 0)
            {
                await DisplayAlert("Error", "Can't find any Soter Wallet device!", "OK");
            }
            else if (SoterDeviceFactoryBle.Instance.Devices.Count == 1)
            {
                device = SoterDeviceFactoryBle.Instance.Devices.FirstOrDefault();
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
                    await device.InitializeAsync();
                    lock (drawStageLock)
                    {
                        drawStage = -1;
                    }
                    Settings.DeviceName = device.Name;
                    await device.InitializeAsync();
                    using (var db = new DatabaseContext())
                    {
                        db.Database.EnsureCreated();
                        foreach (var coinType in await device.GetCoinTableAsync())
                        {
                            if (Settings.SupportedCoins.Any(c => c.Equals(coinType.CoinShortcut)))
                            {
                                db.Coins.Add(new Models.Coin(coinType));
                            }
                        }
                        db.SaveChanges();
                    }

                    Application.Current.MainPage = new NavigationPage(new MainTabbedPage());
                    return;
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
