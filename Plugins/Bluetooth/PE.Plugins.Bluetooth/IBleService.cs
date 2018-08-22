using System;

namespace PE.Plugins.Bluetooth
{
    public enum BleRoles
    {
        Server,
        Client
    }

    public enum BleStates
    {
        Denied,
        Off,
        On
    }

    public interface IBleService
    {
        #region Properties

        IBleServer Server { get; set; }

        IBleClient Client { get; set; }

        #endregion Properties

        #region Operations

        BleStates Available();

        #endregion Operations
    }
}
