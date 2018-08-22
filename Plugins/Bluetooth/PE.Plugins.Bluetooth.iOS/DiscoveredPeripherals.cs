using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PE.Plugins.Bluetooth.iOS
{
    public class DiscoveredPeripherals : ICollection<DiscoveredPeripheral>
    {

        #region Fields

        private List<DiscoveredPeripheral> _List = new List<DiscoveredPeripheral>();
        private int _SecondsValid = 300;

        #endregion Fields

        #region Constructors

        public DiscoveredPeripherals(int secondsValid = 300)
        {
            _SecondsValid = secondsValid;
        }

        #endregion Constructors

        #region Properties

        public int Count => _List.Count;

        public bool IsReadOnly => false;

        #endregion Properties

        public void Add(DiscoveredPeripheral item)
        {
            lock (_List)
            {
                //  only add the item if we don't already have it
                var exists = _List.FirstOrDefault(i => i.Peripheral.UUID.Uuid.Equals(item.Peripheral.UUID.Uuid));
                if (exists == null)
                {
                    _List.Add(item);
                }
                else
                {
                    //  refresh the timestamp
                    exists.TimeStamp = DateTime.Now;
                }
                //  find any expired items and remove them - so that items that are no longer in range don't remain 
                var expired = _List.Where(i => DateTime.Now.Subtract(i.TimeStamp).TotalSeconds > _SecondsValid).ToList();
                if ((expired != null) && (expired.Count > 0))
                {
                    foreach (var ex in expired)
                    {
                        _List.Remove(ex);
                    }
                }
            }
        }

        public void Clear()
        {
            lock(_List)
            {
                _List.Clear();
            }
        }

        public bool Contains(DiscoveredPeripheral item)
        {
            lock(_List)
            {
                return (_List.FirstOrDefault(i => i.Peripheral.UUID.Uuid.Equals(item.Peripheral.UUID.Uuid)) != null);
            }
        }

        public void CopyTo(DiscoveredPeripheral[] array, int arrayIndex)
        {
            throw new NotImplementedException("CopyTo has not been implemented.");
        }

        public IEnumerator<DiscoveredPeripheral> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        public bool Remove(DiscoveredPeripheral item)
        {
            bool removed = false;
            lock(_List)
            {
                //  only add the item if we don't already have it
                var p = _List.FirstOrDefault(i => i.Peripheral.UUID.Uuid.Equals(item.Peripheral.UUID.Uuid));
                if (p != null) removed = _List.Remove(p);
            }
            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _List.GetEnumerator();
        }
    }
}