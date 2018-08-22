namespace PE.Plugins.Bluetooth
{
    public class BleCharacteristic
    {
        public string Name { get; set; }

        public string Guid { get; set; }

        public ushort GuidValue { get; set; }

        public bool IsStatic { get; set; } = false;

        public object StaticValue { get; set; }
    }
}
