using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace PE.Plugins.Bluetooth.WindowsCommon
{
    public class BleClient : IBleClient
    {
        #region Events

        public event EventHandler<DeviceEventArgs> DeviceFound;
        public event EventHandler<DeviceEventArgs> DeviceConnected;
        public event EventHandler<CharacteristicEventArgs> CharacteristicFound;
        public event EventHandler ServiceError;

        #endregion Events

        #region Fields

        private DeviceWatcher _Watcher;

        private List<DeviceInformation> _Discovered = new List<DeviceInformation>();
        private List<BluetoothLEDevice> _Connected = new List<BluetoothLEDevice>();

        #endregion Fields

        #region Operations

        public bool ConnectDevice(BleDevice device)
        {
            return Task.Run(async () => await ConnectDeviceAsync(device)).Result;
        }

        private async Task<bool> ConnectDeviceAsync(BleDevice device)
        {
            try
            {
                var bleDevice = await BluetoothLEDevice.FromIdAsync(device.Guid);
                _Connected.Add(bleDevice);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BleClient.ConnectDeviceAsync - Exception: {0}", ex);
                return false;
            }
        }

        public void DisconnectDevice(BleDevice device)
        {
            var exists = _Connected.FirstOrDefault(d => d.DeviceId.Equals(device.Guid));
            if (exists == null) return;
            exists.Dispose();
            _Connected.Remove(exists);
            exists = null;
        }

        public void StartScan(BleDevice device)
        {
            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            _Watcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false), requestedProperties, DeviceInformationKind.AssociationEndpoint);

            //  register event handlers before starting the watcher.
            _Watcher.Added += DeviceWatcher_Added;
            _Watcher.Updated += DeviceWatcher_Updated;
            _Watcher.Removed += DeviceWatcher_Removed;
            _Watcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            _Watcher.Start();
        }

        public void StopScan()
        {
            if (_Watcher == null) return;
            _Watcher.Stop();
        }

        #endregion Operations

        #region Event Handlers

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            _Watcher.Added -= DeviceWatcher_Added;
            _Watcher.Updated -= DeviceWatcher_Updated;
            _Watcher.Removed -= DeviceWatcher_Removed;
            _Watcher.Stopped -= DeviceWatcher_Stopped;
            _Watcher = null;
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var exists = _Discovered.FirstOrDefault(d => d.Id.Equals(args.Id));
            if (exists == null) return;
            _Discovered.Remove(exists);
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var exists = _Discovered.FirstOrDefault(d => d.Id.Equals(args.Id));
            if (exists == null) return;
            DeviceFound?.Invoke(this, new DeviceEventArgs { Device = new BleDevice { Guid = args.Id, Name = exists.Name } });
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var exists = _Discovered.FirstOrDefault(d => d.Id.Equals(args.Id));
            if (exists != null) return;
            _Discovered.Add(args);
            DeviceFound?.Invoke(this, new DeviceEventArgs { Device = new BleDevice { Guid = args.Id, Name = args.Name } });
        }

        #endregion Event Handlers
    }
}
