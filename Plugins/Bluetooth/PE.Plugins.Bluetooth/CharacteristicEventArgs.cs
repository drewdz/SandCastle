using System;

namespace PE.Plugins.Bluetooth
{
    public class CharacteristicEventArgs : EventArgs
    {
        public BleCharacteristic Characteristic { get; set; }
    }
}
