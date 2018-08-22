namespace PE.Plugins.Bluetooth.WindowsCommon
{
    public class BleService : IBleService
    {
        #region Properties

        public IBleServer Server { get; set; } = new BleServer();
        public IBleClient Client { get; set; } = new BleClient();

        #endregion Properties

        #region Operations

        public bool Available()
        {
            //  TODO: Check whether BLE is supported and available
            return true;
        }

        BleStates IBleService.Available()
        {
            throw new System.NotImplementedException();
        }

        #endregion Operations
    }
}
