using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace SoterWalletMobile.ViewModels
{
    public class WalletViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Shortcut { get; set; }

        string _balance;
        public string Balance
        {
            get { return _balance; }
            set
            {
                if (!String.Equals(_balance, value))
                {
                    _balance = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string _balanceFiat;
        public string BalanceFiat
        {
            get { return _balanceFiat; }
            set
            {
                if (!String.Equals(_balanceFiat, value))
                {
                    _balanceFiat = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ImageSource Icon { get; set; }

        public string DefaultAddress { get; set; }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
