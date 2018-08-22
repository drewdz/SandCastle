using CoreBluetooth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PE.Plugins.Bluetooth.iOS
{
    public class BleClient : IBleClient, ICBPeripheralManagerDelegate, ICBCentralManagerDelegate
    {
        #region Events

        public event EventHandler<DeviceEventArgs> DeviceFound;
        public event EventHandler<DeviceEventArgs> DeviceConnected;
        public event EventHandler<CharacteristicEventArgs> CharacteristicFound;
        public event EventHandler ServiceError;

        #endregion Events

        #region Fields

        private bool _Disposed = false;

        private CBPeripheralManager _PeripheralManager;
        private CBMutableService _Service;
        private List<CBMutableCharacteristic> _Characteristics;

        private CBCentralManager _CentralManager;

        //  SERVER
        //  Generic Access	org.bluetooth.service.generic_access	0x1800
        //  Automation IO	org.bluetooth.service.automation_io	0x1815
        //  Human Interface Device	org.bluetooth.service.human_interface_device	0x1812
        //  Scan Parameters	org.bluetooth.service.scan_parameters	0x1813

        private readonly CBUUID _ServiceId = CBUUID.FromPartial(0x1815);

        //  CHARACTERISTICS
        //  Object Changed	org.bluetooth.characteristic.object_changed	0x2AC8
        private readonly CBUUID _ChangedId = CBUUID.FromPartial(0x2AC8);

        private DiscoveredPeripherals _Discovered = new DiscoveredPeripherals();
        private List<CBPeripheral> _Connected = new List<CBPeripheral>();

        #endregion Fields

        #region Properties

        public IntPtr Handle
        {
            get { return (_CentralManager == null) ? IntPtr.Zero : _CentralManager.Handle; }
        }

        #endregion Properties

        #region Operations

        public void StartScan(BleDevice device)
        {
            _CentralManager = new CBCentralManager(this, null);
            if (device == null) throw new Exception("Could not start client without a service type to scan for.");
            _CentralManager.DiscoveredPeripheral += (o, e) =>
            {
                DeviceFound?.Invoke(this, new DeviceEventArgs { Device = new BleDevice { Guid = e.Peripheral.UUID.Uuid } });
            };
            _CentralManager.ConnectedPeripheral += (o, e) =>
            {
                DeviceConnected?.Invoke(this, new DeviceEventArgs { Device = new BleDevice { Guid = e.Peripheral.UUID.Uuid } });
            };
            //_CentralManager.ScanForPeripherals(CBUUID.FromPartial(device.GuidValue));
            _CentralManager.ScanForPeripherals(CBUUID.FromString(device.Guid));
        }

        public void StopScan()
        {
            _CentralManager.StopScan();
        }

        public bool ConnectDevice(BleDevice device)
        {
            //  find the device in the list
            var discovered = _Discovered.FirstOrDefault(d => d.Peripheral.UUID.Uuid.Equals(device.Guid));
            if (discovered == null) return false;
            //  connect the device
            _CentralManager.ConnectPeripheral(discovered.Peripheral);
            _Connected.Add(discovered.Peripheral);
            return true;
        }

        public void DisconnectDevice(BleDevice device)
        {
            var peripheral = _Connected.FirstOrDefault(p => p.UUID.Uuid.Equals(device.Guid));
            if (peripheral == null) return;
            //  TODO: disconnect
        }

        #endregion Operations

        #region Callbacks

        /// <summary>
        /// Called to update state of the peripheral manager
        /// </summary>
        /// <param name="peripheral"></param>
        public void StateUpdated(CBPeripheralManager peripheral)
        {
        }

        /// <summary>
        /// Called to update state of the central manager
        /// </summary>
        /// <param name="central"></param>
        public void UpdatedState(CBCentralManager central)
        {
        }

        private void GetServices(CBPeripheral peripheral)
        {
            peripheral.DiscoveredService += (o, e) =>
            {
            };
            peripheral.DiscoverServices();
        }

        #endregion Callbacks

        #region Cleanup

        public void Dispose()
        {
            if (_Disposed) return;
            //  cleanup resources
            if (_PeripheralManager != null) _PeripheralManager.Dispose();
            _Disposed = true;
        }

        #endregion Cleanup
    }
}