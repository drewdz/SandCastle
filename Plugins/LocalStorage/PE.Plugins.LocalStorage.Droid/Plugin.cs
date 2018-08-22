using System;
using MvvmCross;
using MvvmCross.Plugin;
using PE.Framework.Droid.AndroidApp.AppVersion;

namespace PE.Plugins.LocalStorage.Droid
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        #region Fields

        private LocalStorageConfiguration _Configuration;

        #endregion Fields

        #region Plugin

        public void Configure(IMvxPluginConfiguration configuration)
        {
            if (!(configuration is LocalStorageConfiguration)) throw new Exception("Configuration does not appear to be a valid NetworkConfiguration.");
            _Configuration = (LocalStorageConfiguration)configuration;
        }

        public void Load()
        {
            IAndroidApp androidApp = Mvx.Resolve<IAndroidApp>();
            _Configuration.ApplicationContext = androidApp.TopActivity.ApplicationContext;
            Mvx.LazyConstructAndRegisterSingleton<ILocalStorageService>(() => new LocalStorageService(_Configuration));
        }

        #endregion Plugin
    }
}
