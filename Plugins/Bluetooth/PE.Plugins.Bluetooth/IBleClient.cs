using System;

namespace PE.Plugins.Bluetooth
{
    public interface IBleClient
    {
        #region Events

        event EventHandler<DeviceEventArgs> DeviceFound;
        event EventHandler<DeviceEventArgs> DeviceConnected;
        event EventHandler<CharacteristicEventArgs> CharacteristicFound;
        event EventHandler ServiceError;

        #endregion Events

        #region Operations

        void StartScan(BleDevice device);

        void StopScan();

        bool ConnectDevice(BleDevice device);

        void DisconnectDevice(BleDevice device);

        #endregion Operations
    }
}
