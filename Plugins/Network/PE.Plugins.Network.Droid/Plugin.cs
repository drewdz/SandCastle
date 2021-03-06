﻿using MvvmCross;
using MvvmCross.Plugin;

using PE.Plugins.Network.Contracts;

using System;

namespace PE.Plugins.Network.Droid
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        #region Fields

		private NetworkConfigurationDroid _Configuration;

        #endregion Fields

        #region Plugin

        public void Configure(IMvxPluginConfiguration configuration)
        {
			if (!(configuration is NetworkConfigurationDroid)) throw new Exception("Configuration does not appear to be a valid NetworkConfiguration.");
            _Configuration = (NetworkConfigurationDroid)configuration;
        }

        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<INetworkService>(() => new NetworkService(_Configuration));
            Mvx.LazyConstructAndRegisterSingleton<IRestService>(() => new RestService(_Configuration));
        }

        #endregion Plugin
    }
}
