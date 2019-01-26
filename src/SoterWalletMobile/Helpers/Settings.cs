﻿using Plugin.Settings;
using Plugin.Settings.Abstractions;
namespace SoterWalletMobile.Helpers
{
    public static class Settings
    {
        public static readonly string[] SupportedCoins = { "BTC", "DODGE", "ETH", "LTC", "BCH" };

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private const string DeviceNameKey = "device_name";
        public static string DeviceName
        {
            get
            {
                return AppSettings.GetValueOrDefault(DeviceNameKey, string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue(DeviceNameKey, value);
            }
        }

    }
}
