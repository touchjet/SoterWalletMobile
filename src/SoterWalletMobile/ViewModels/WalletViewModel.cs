using Xamarin.Forms;

namespace SoterWalletMobile.ViewModels
{
    public class WalletViewModel
    {
        public string Name { get; set; }
        public string Shortcut { get; set; }
        public string Balance { get; set; }
        public string BalanceFiat { get; set; }
        public ImageSource Icon { get; set; }
    }
}
