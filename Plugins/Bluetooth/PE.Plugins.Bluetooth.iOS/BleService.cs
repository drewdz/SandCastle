using CoreBluetooth;

namespace PE.Plugins.Bluetooth.iOS
{
    //  SERVER
    //  Generic Access	org.bluetooth.service.generic_access	0x1800
    //  Automation IO	org.bluetooth.service.automation_io	0x1815
    //  Human Interface Device	org.bluetooth.service.human_interface_device	0x1812
    //  Scan Parameters	org.bluetooth.service.scan_parameters	0x1813

    //  CHARACTERISTICS
    //  Object Changed	org.bluetooth.characteristic.object_changed	0x2AC8

    public class BleService : IBleService
    {
        #region Properties

        public IBleServer Server { get; set; } = new BleServer();

        public IBleClient Client { get; set; } = new BleClient();

        #endregion Properties

        #region Operations

        public BleStates Available()
        {
            var manager = new CBPeripheralManager();
            //  already authorized
            if (CBPeripheralManager.AuthorizationStatus == CBPeripheralManagerAuthorizationStatus.Authorized)
            {
                return (manager.State == CBPeripheralManagerState.PoweredOn) ? BleStates.On : BleStates.Off;
            }
            //  request auth
            manager.respo

            return true;
        }

        #endregion Operations
    }
}