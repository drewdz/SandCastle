﻿using MvvmCross;
using MvvmCross.Plugin;

using PE.Plugins.Network.Contracts;

using System;

namespace PE.Plugins.Network.WindowsCommon
{
    [MvxPlugin]
    public class Plugin : IMvxConfigurablePlugin
    {
        #region Fields

        private NetworkConfiguration _Configuration { get; set; }

        #endregion Fields

        #region Plugin

        #endregion Plugin

        public void Configure(IMvxPluginConfiguration configuration)
        {
            if (!(configuration is NetworkConfiguration)) throw new Exception("Configuration does not appear to be a valid NetworkConfiguration.");
            _Configuration = (NetworkConfiguration)configuration;
        }

        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<INetworkService>(() => new NetworkService(_Configuration));
            Mvx.LazyConstructAndRegisterSingleton<IRestService>(() => new RestService(_Configuration));
		}
    }
}
