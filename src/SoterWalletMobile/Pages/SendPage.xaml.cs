using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BlockchainService.Abstractions.Models;
using Newtonsoft.Json;
using Serilog;
using SoterDevice.Contracts;
using SoterDevice.Models;
using SoterWalletMobile.Data;
using SoterWalletMobile.Services;
using SoterWalletMobile.ViewModels;
using Touchjet.BinaryUtils;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class SendPage : ContentPage
    {
        ObservableCollection<WalletViewModel> walletViewModels;

        WalletViewModel selectedCoin;

        public SendPage()
        {
            InitializeComponent();
            walletViewModels = Repository.GetWalletViewModels();
            coinListView.ItemsSource = walletViewModels;
            coinListView.SelectedItem = walletViewModels.FirstOrDefault();
        }

        void SelectCoin(WalletViewModel selected)
        {
            selectedCoin = selected;
            labelBalance.Text = selected.Balance;
            labelCurrency.Text = selected.Shortcut;
            amountEntry.Text = string.Empty;
            toAddressEntry.Text = string.Empty;
        }

        void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(toAddressEntry.Text)||string.IsNullOrWhiteSpace(amountEntry.Text))
            {
                nextButton.IsEnabled = false;
                nextButton.Style = (Style)Application.Current.Resources["disabledButtonStyle"];
            }
            else
            {
                nextButton.IsEnabled = true;
                nextButton.Style = (Style)Application.Current.Resources["normalButtonStyle"];
            }
        }

        async void ScanButton_Tapped(object sender, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                toAddressEntry.Text = result.Text;
            }
        }

        async void NextButton_Clicked(object sender, EventArgs e)
        {
            var bitcoinTransaction = new BitcoinTransaction(selectedCoin.Name);

            var bitcoinService = BitcoinService.GetBitcoinService(selectedCoin.Shortcut);
            var tx = await bitcoinService.CreateTransactionAsync(new BitcoinTX()
            {
                Inputs = new List<BitcoinTXInput> { new BitcoinTXInput { Addresses = new List<string> { selectedCoin.DefaultAddress } } },
                Outputs = new List<BitcoinTXOutput> { new BitcoinTXOutput { Addresses = new List<string> { toAddressEntry.Text }, Value = 100000 } }
            });

            foreach (var input in tx.Tx.Inputs)
            {
                var newInput = new BitcoinTransactionInput
                {
                    AddressNs = Repository.GetAddress(input.Addresses[0]).GetAddressPath(),
                    Amount = (ulong)input.OutputValue,
                    PrevHash = input.PrevHash.ToBytes(),
                    PrevIndex = (uint)input.OutputIndex,
                    Sequence = (uint)input.Sequence
                };
                var inputTx = await bitcoinService.GetTransactionAsync(input.PrevHash);

                newInput.PrevTransaction = new BitcoinTransaction(selectedCoin.Shortcut, (uint)inputTx.Ver, (uint)inputTx.LockTime);

                foreach (var prevInput in inputTx.Inputs)
                {
                    newInput.PrevTransaction.Inputs.Add(new BitcoinTransactionInput
                    {
                        PrevHash = prevInput.PrevHash.ToBytes(),
                        ScriptSig = prevInput.Script.ToBytes(),
                        PrevIndex = (uint)prevInput.OutputIndex,
                        Sequence = (uint)prevInput.Sequence,
                        ScriptType = InputScriptType.Spendaddress
                    });
                }
                foreach (var prevOutput in inputTx.Outputs)
                {
                    newInput.PrevTransaction.Outputs.Add(new BitcoinTransactionOutput
                    {
                        Amount = (ulong)prevOutput.Value,
                        Script = prevOutput.Script.ToBytes()
                    });
                }

                bitcoinTransaction.Inputs.Add(newInput);
            }

            foreach (var output in tx.Tx.Outputs)
            {
                bitcoinTransaction.Outputs.Add(new BitcoinTransactionOutput
                {
                    Address = output.Addresses[0],
                    Script = output.Script.ToBytes(),
                    Amount = (ulong)output.Value,
                    AddressType = OutputAddressType.Spend,
                    ScriptType = OutputScriptType.Paytoscripthash,
                });
            }

            var signedTx = await DeviceCommPage.SignBitcoinTransaction(this, bitcoinTransaction);
            Log.Debug(JsonConvert.SerializeObject(signedTx));
            var txPushResult = await bitcoinService.PushRawTransactionAsync(new BitcoinTXRaw { Tx = signedTx.SerializedTx.ToHex().ToLower() });
            Log.Debug(JsonConvert.SerializeObject(txPushResult));
        }

        void CoinList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SelectCoin((WalletViewModel)e.SelectedItem);
        }
    }
}
