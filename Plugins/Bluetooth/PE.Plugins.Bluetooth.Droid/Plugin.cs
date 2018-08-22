using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.Bluetooth.Droid
{
    [MvxPlugin]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            //Mvx.LazyConstructAndRegisterSingleton<IBleService>(() => BleService());
        }
    }
}
