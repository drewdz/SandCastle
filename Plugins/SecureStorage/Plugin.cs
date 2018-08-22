using MvvmCross;
using MvvmCross.Plugin;

namespace PE.Plugins.SecureStorage
{
	[MvxPlugin]
	public class Plugin : IMvxConfigurablePlugin
    {
		#region Fields

		private const string _AppName = "Atelier";
		private SecureStorageConfiguration _Configuration;

        #endregion Fields

        #region Plugin

        public void Configure(IMvxPluginConfiguration configuration)
        {
			if (!(configuration is SecureStorageConfiguration)) throw new System.Exception("Configuration does not appear to be a valid SecureStorageConfiguration.");
			_Configuration = (SecureStorageConfiguration)configuration;
        }

        public void Load()
        {
			Mvx.LazyConstructAndRegisterSingleton<ISessionService>(() => new SessionService(_Configuration, _AppName));
        }

        #endregion Plugin
    }
}
