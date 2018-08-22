using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.Bluetooth.WindowsCommon
{
    [MvxPlugin]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            //Mvx.LazyConstructAndRegisterSingleton<IBleService>(() => new BleService());
        }
    }
}
