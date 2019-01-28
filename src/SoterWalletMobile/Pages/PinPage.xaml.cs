using System;
using Serilog;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SoterDevice.Contracts;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class PinPage : ContentPage
    {
        public static string PIN = String.Empty;

        public PinPage()
        {
            InitializeComponent();
        }

        public PinPage(PinMatrixRequestType type)
        {
            InitializeComponent();
            switch (type)
            {
                case PinMatrixRequestType.PinMatrixRequestTypeCurrent:
                    titleLabel.Text = AppResources.EnterCurrentPin;
                    break;
                case PinMatrixRequestType.PinMatrixRequestTypeNewFirst:
                    titleLabel.Text = AppResources.EnterNewPin;
                    break;
                case PinMatrixRequestType.PinMatrixRequestTypeNewSecond:
                    titleLabel.Text = AppResources.ReEnterCurrentPin;
                    break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PIN = String.Empty;
        }

        void PIN_Clicked(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            PIN = PIN + button.ClassId;
            canvasView.InvalidateSurface();
        }

        void Cancel_Clicked(object sender, System.EventArgs e)
        {
            PIN = String.Empty;
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
