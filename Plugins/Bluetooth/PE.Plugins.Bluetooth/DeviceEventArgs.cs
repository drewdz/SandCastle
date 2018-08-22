using System;

namespace PE.Plugins.Bluetooth
{
    public class DeviceEventArgs : EventArgs
    {
        #region Properties

        public BleDevice Device { get; set; }

        #endregion Properties
    }
}
