namespace PE.Plugins.Bluetooth
{
    public interface IBleServer
    {
        void Start(BleDevice device, BleCharacteristic[] characteristics);
    }
}
