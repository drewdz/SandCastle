using CoreBluetooth;
using CoreFoundation;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PE.Plugins.Bluetooth.iOS
{
    public class BleServer : NSObject, IBleServer, ICBPeripheralManagerDelegate
    {
        #region Fields

        private bool _Disposed = false;

        private CBPeripheralManager _PeripheralManager;
        private CBMutableService _Service;
        private List<CBMutableCharacteristic> _Characteristics;

        private CBPeripheralManager peripheralManager;

        #endregion Fields

        #region Properties

        public IntPtr Handle
        {
            get
            {
                return (_PeripheralManager == null) ? IntPtr.Zero : _PeripheralManager.Handle;
            }
        }

        #endregion Properties

        #region Operations

        public void Start_old(BleDevice device, BleCharacteristic[] characteristics)
        {
            try
            {
                if (device == null) throw new Exception("Could not initialize server without a service description.");
                if ((characteristics == null) || (characteristics.Length == 0)) throw new Exception("Could not initialize server without characteristic descriptions.");

                //  setup a peripheral manager
                _PeripheralManager = new CBPeripheralManager();
                var uuid = CBUUID.FromPartial(device.GuidValue);
                System.Diagnostics.Debug.WriteLine(string.Format("*** BleServer.Start - Device Guid: {0}", uuid.Uuid));
                _Service = new CBMutableService(uuid, true);
                _Characteristics = characteristics.Select(ch => new CBMutableCharacteristic(CBUUID.FromPartial(ch.GuidValue), CBCharacteristicProperties.Read, null, CBAttributePermissions.Readable)).ToList();
                _Service.Characteristics = _Characteristics.ToArray();

                //  register services
                _PeripheralManager.AddService(_Service);
                _PeripheralManager.AdvertisingStarted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("*** BleServer -> Advertising error: {0}", e.Error.Description));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*** BleServer -> We are advertising.");
                    }
                };

                //  advertise services
                var opt = new StartAdvertisingOptions();
                opt.ServicesUUID = new CBUUID[] { _Service.UUID };
                _PeripheralManager.StartAdvertising(opt);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** BleServer.Start - Exception: {0}", ex));
            }
        }

        public void StateUpdated(CBPeripheralManager peripheral)
        {
            throw new NotImplementedException();
        }

        public void Start(BleDevice device, BleCharacteristic[] characteristics)
        {
            string dataServiceUUIDsKey = "71DA3FD1-7E10-41C1-B16F-4430B5060000";
            string customBeaconServiceUUIDsKey = "71DA3FD1-7E10-41C1-B16F-4430B5060001";
            string customBeaconCharacteristicUUIDKey = "71DA3FD1-7E10-41C1-B16F-4430B5060002";
            string identifier = "71DA3FD1-7E10-41C1-B16F-4430B5060003";

            peripheralManager = new CBPeripheralManager(this, DispatchQueue.DefaultGlobalQueue);
            peripheralManager.AdvertisingStarted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("*** BleServer -> Advertising error: {0}", e.Error.Description));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("*** BleServer -> We are advertising.");
                }
            };


            var customBeaconServiceUUID = CBUUID.FromString(customBeaconServiceUUIDsKey);
            var customBeaconCharacteristicUUID = CBUUID.FromString(customBeaconCharacteristicUUIDKey);

            var service = new CBMutableService(customBeaconServiceUUID, true);
            var dataUUID = NSData.FromString(identifier, NSStringEncoding.UTF8);

            var characteristic = new CBMutableCharacteristic(
                customBeaconCharacteristicUUID,
                CBCharacteristicProperties.Read,
                dataUUID,
                CBAttributePermissions.Readable);
            service.Characteristics = new CBCharacteristic[] { characteristic };
            peripheralManager.AddService(service);

            var localName = new NSString("CustomBeacon");

            //var advertisingData = new NSDictionary( CBAdvertisement.DataLocalNameKey, localName,
            //    CBAdvertisement.IsConnectable, false,
            //    CBAdvertisement.DataManufacturerDataKey, CBUUID.FromString(dataServiceUUIDsKey),
            //    CBAdvertisement.DataServiceUUIDsKey, CBUUID.FromString(dataServiceUUIDsKey));

            //peripheralManager.StartAdvertising(advertisingData);




            var UUI = new CBUUID[] { CBUUID.FromString("71DA3FD1-7E10-41C1-B16F-4430B5060000") };

            NSArray arry = NSArray.FromObjects(UUI);
            var test = NSObject.FromObject(arry);
            var ad = new NSDictionary( CBAdvertisement.DataServiceUUIDsKey, test);

            peripheralManager.StartAdvertising(ad);
        }

        public void Dispose()
        {
            if (_PeripheralManager == null) return;
            _PeripheralManager.Dispose();
            _PeripheralManager = null;
        }

        #endregion Operations
    }
}