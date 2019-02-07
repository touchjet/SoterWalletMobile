using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SoterDevice.Ble;
using SoterDevice.Models;
using SoterWalletMobile.Data;
using Touchjet.BinaryUtils;
using Xamarin.Forms;

namespace SoterWalletMobile.Pages
{
    public partial class DeviceCommPage : ContentPage
    {
        static Page _parentPage;
        static EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        static BitcoinTransaction _bitcoinTX;
        static string _deviceLabel;

        static Dictionary<CommFunction, List<CommState>> _processes = new Dictionary<CommFunction, List<CommState>> {
            { CommFunction.WipeDevice, new List<CommState> { CommState.WipeDevice } },
            { CommFunction.ResetDevice, new List<CommState>{ CommState.ResetDevice, CommState.UpdateCoinTable}},
            { CommFunction.UpdateCoinTable,new List<CommState>{ CommState.UpdateCoinTable}},
            { CommFunction.SignBitcoinTx,new List<CommState>{ CommState.SignBitcoinTx}},
        };


        bool _reEntry;
        CommState _state;

        public static async Task<BitcoinTransaction> SignBitcoinTransaction(Page parentPage, BitcoinTransaction bitcoinTX)
        {
            _parentPage = parentPage;
            _bitcoinTX = bitcoinTX;
            _waitHandle.Reset();
            var deviceCommPage = new DeviceCommPage(CommFunction.SignBitcoinTx);
            await _parentPage.Navigation.PushModalAsync(deviceCommPage);
            await Task.Run(() => _waitHandle.WaitOne());
            return _bitcoinTX;
        }

        public static async Task ResetDevice(Page parentPage, string deviceLabel)
        {
            _parentPage = parentPage;
            _deviceLabel = deviceLabel;
            _waitHandle.Reset();
            var deviceCommPage = new DeviceCommPage(CommFunction.ResetDevice);
            await _parentPage.Navigation.PushModalAsync(deviceCommPage);
            await Task.Run(() => _waitHandle.WaitOne());
        }

        public static async Task UpdateCoinTable(Page parentPage)
        {
            _parentPage = parentPage;
            _waitHandle.Reset();
            var deviceCommPage = new DeviceCommPage(CommFunction.UpdateCoinTable);
            await _parentPage.Navigation.PushModalAsync(deviceCommPage);
            await Task.Run(() => _waitHandle.WaitOne());
        }

        List<CommState> _selectedProcess;
        int _processStep;

        public DeviceCommPage()
        {
            InitializeComponent();
        }

        DeviceCommPage(CommFunction function)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _selectedProcess = _processes[function];
            _processStep = 0;
            _reEntry = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!_reEntry)
            {
                _reEntry = true;
                var device = SoterDeviceFactoryBle.Instance.CurrentDevice;
                if ((device ==null)||(!device.Connected))
                {
                    if (!await ConnectDevicePage.Connect(this))
                    {
                        await Navigation.PopModalAsync();
                        return;
                    }
                }
                try
                {
                    device = SoterDeviceFactoryBle.Instance.CurrentDevice;
                    deviceName.Text = device.Name;
                    PinPage.ParentPage = this;
                    device.DeviceButtonRequestCallback += Device_DeviceButtonRequestCallback;
                    try
                    {
                        while (_processStep < _selectedProcess.Count)
                        {
                            var currentState = _selectedProcess[_processStep];
                            switch (currentState)
                            {
                                case CommState.WipeDevice:
                                    messageLabel.Text = AppResources.ConfirmWipeMessage;
                                    await SoterDeviceFactoryBle.Instance.CurrentDevice.WipeDeviceAsync();
                                    _state = CommState.ResetDevice;
                                    break;
                                case CommState.ResetDevice:
                                    messageLabel.Text = AppResources.InitializeDeviceMessage;
                                    await SoterDeviceFactoryBle.Instance.CurrentDevice.ResetDeviceAsync(_deviceLabel);
                                    _state = CommState.UpdateCoinTable;
                                    break;
                                case CommState.UpdateCoinTable:
                                    messageLabel.Text = AppResources.LoadingMessage;
                                    await device.InitializeAsync();
                                    Repository.SaveCurrentDeviceToDb(device);
                                    await Repository.LoadCoinTableFromDeviceAsync(device);
                                    break;
                                case CommState.SignBitcoinTx:
                                    await device.InitializeAsync();
                                    _bitcoinTX.SerializedTx = await device.SignTransactionAsync(_bitcoinTX);
                                    break;
                            }
                            _processStep++;
                        }
                    }
                    finally
                    {
                        PinPage.ParentPage = null;
                        device.DeviceButtonRequestCallback -= Device_DeviceButtonRequestCallback;
                        device.Disconnect();
                        await Navigation.PopModalAsync();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
                _waitHandle.Set();
            }
        }

        void Device_DeviceButtonRequestCallback()
        {
            switch (_state)
            {
                case CommState.ResetDevice:
                    messageLabel.Text = AppResources.RecoverSentenceMessage;
                    break;
            }
        }

    }

    enum CommFunction
    {
        WipeDevice,
        ResetDevice,
        UpdateCoinTable,
        SignBitcoinTx
    }

    enum CommState
    {
        WipeDevice,
        ResetDevice,
        UpdateCoinTable,
        SignBitcoinTx
    }
}
