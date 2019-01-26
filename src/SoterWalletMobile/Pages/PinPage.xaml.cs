using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class PinPage : ContentPage
    {
        public static string PIN = "";

        public PinPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PIN = "";
        }

        void PIN_Clicked(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            Log.Information($"PIN {button.ClassId}");
            PIN = PIN + button.ClassId;
            canvasView.InvalidateSurface();
        }

        void Cancel_Clicked(object sender, System.EventArgs e)
        {
            PIN = "";
            canvasView.InvalidateSurface();
        }

        async void Confirm_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            using (SKPaint paint = new SKPaint())
            {
                int unit = info.Width / 188;
                int circleCentreX = 6 * unit;
                for (uint i = 0; i < 9; i++)
                {
                    if (PIN.Length > i)
                    {
                        paint.Color = new SKColor(0xff416aff);
                        paint.Style = SKPaintStyle.StrokeAndFill;
                        paint.StrokeWidth = unit;
                        canvas.DrawCircle(circleCentreX, 6 * unit, 5 * unit, paint);
                    }
                    else
                    {
                        paint.Color = new SKColor(0xffe4e6ed);
                        paint.Style = SKPaintStyle.Stroke;
                        paint.StrokeWidth = unit;
                        canvas.DrawCircle(circleCentreX, 6 * unit, 5 * unit, paint);
                    }
                    circleCentreX += 22 * unit;
                }
            }
        }
    }
}
