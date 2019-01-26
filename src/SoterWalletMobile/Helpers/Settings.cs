using Plugin.Settings;
using Plugin.Settings.Abstractions;
namespace SoterWalletMobile.Helpers
{
    public static class Settings
    {
        public static readonly string[] SupportedCoins = { "BTC", "TEST", "DODGE", "ETH", "LTC", "BCH" };

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

        private const string DeviceIdKey = "device_id";
        public static string DeviceId
        {
            get
            {
                return AppSettings.GetValueOrDefault(DeviceIdKey, string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue(DeviceIdKey, value);
            }
        }

    }
}
