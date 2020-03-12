using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using System;
using System.Globalization;

namespace InstagramApiSharp.Helpers
{
    public static class ConnectionSpeedGenerator
    {
        private static Random random = new Random();
        public static string GenerateRandomBandwithSpeed()
        {
            return $"{random.Next(InstaApiConstants.MIN_BANDWIDTH_SPEED_KBPS, InstaApiConstants.MAX_BANDWIDTH_SPEED_KBPS)}.{random.Next(InstaApiConstants.MIN_BANDWIDTH_SPEED_BPS, InstaApiConstants.MAX_BANDWIDTH_SPEED_BPS)}";
        }

        public static string GenerateRandomBandwidthTotalTime()
        {
            return $"{random.Next(InstaApiConstants.MIN_BANDWIDTH_TOTAL_TIME_MS, InstaApiConstants.MAX_BANDWIDTH_TOTAL_TIME_MS)}";
        }

        public static string GenerateBandwithTotalBytes(string bandwidthSpeedKbps, string bandwidthTotalTimeMS)
        {
            double speed = double.Parse(bandwidthSpeedKbps, NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            int time = Int32.Parse(bandwidthTotalTimeMS);
            var response = (Convert.ToInt32(speed * time + random.Next(100, 999))).ToString();
            return response;
        }

        public static AndroidDevice RandomizeBandwithConnection(this AndroidDevice device)
        {
            device.IGBandwidthSpeedKbps = GenerateRandomBandwithSpeed();
            device.IGBandwidthTotalTimeMS = GenerateRandomBandwidthTotalTime();
            device.IGBandwidthTotalBytesB = GenerateBandwithTotalBytes(device.IGBandwidthSpeedKbps, device.IGBandwidthTotalTimeMS);

            return device;
        }
    }
}
