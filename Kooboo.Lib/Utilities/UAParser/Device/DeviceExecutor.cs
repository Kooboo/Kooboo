namespace Kooboo.Lib.Utilities.UAParser.Device
{
    public class DeviceExecutor
    {
        private const string Others = "Others";
        private readonly IDeviceMatcher[] _deviceMatchers;

        public DeviceExecutor()
        {
            _deviceMatchers = new IDeviceMatcher[]
            {
                new MiDeviceMatcher(),
                new HuaweiDeviceMatcher(),
                new HonorDeviceMatcher(),
                new SamsungDeviceMatcher(),
                new SonyDeviceMatcher(),
                new AppleDeviceMatcher(),
                new OnePlusDeviceMatcher(),
                new OppoDeviceMatcher(),
                new VivoDeviceMatcher(),
                new VertuDeviceMatcher(),
                new MeizuDeviceMatcher()

            };
        }

        public string Match(string userAgentString)
        {
            foreach (var item in _deviceMatchers)
            {
                if (item.Match(userAgentString))
                    return item.DeviceName;
            }

            return Others;
        }
    }
}

