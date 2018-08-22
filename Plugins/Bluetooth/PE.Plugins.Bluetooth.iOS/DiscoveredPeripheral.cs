using CoreBluetooth;
using System;

namespace PE.Plugins.Bluetooth.iOS
{
    public class DiscoveredPeripheral
    {
        public DateTime TimeStamp { get; set; }

        public CBPeripheral Peripheral { get; set; }
    }
}