using MvvmCross.Commands;
using MvvmCross.ViewModels;
//using PE.Plugins.Bluetooth;
using System;
using System.Collections.Generic;

namespace SandCastle.Core.ViewModels
{
    //  SERVER
    //  Generic Access	org.bluetooth.service.generic_access	0x1800
    //  Automation IO	org.bluetooth.service.automation_io	0x1815
    //  Human Interface Device	org.bluetooth.service.human_interface_device	0x1812
    //  Scan Parameters	org.bluetooth.service.scan_parameters	0x1813

    //  CHARACTERISTICS
    //  Object Changed	org.bluetooth.characteristic.object_changed	0x2AC8
    public class ClientViewModel : MvxViewModel
    {
        #region Fields

        //private readonly IBleService _BleService;

        #endregion Fields

        #region Constructors

        public ClientViewModel()
        {
            //_BleService = bleService;

            //  listen for changes
        }

        #endregion Constructors

        #region Properties

        //private List<BleDevice> _Devices = new List<BleDevice>();
        //public List<BleDevice> Devices
        //{
        //    get => _Devices;
        //    set => SetProperty(ref _Devices, value);
        //}

        //private BleDevice _Device = null;
        //public BleDevice Device
        //{
        //    get => _Device;
        //    set => SetProperty(ref _Device, value);
        //}

        #endregion Properties

        #region Commands

        private IMvxCommand _ScanCommand = null;
        public IMvxCommand ScanCommand => _ScanCommand ?? new MvxCommand(() => Start());

        private IMvxCommand _StopCommand = null;
        public IMvxCommand StopCommand => _StopCommand ?? new MvxCommand(() =>
        {
            //_BleService.Client.StopScan();
        });

        private IMvxCommand _AddCommand = null;
        public IMvxCommand AddCommand => _AddCommand ?? new MvxCommand(() =>
        {
        });

        #endregion Commands

        #region Actions

        private void Start()
        {
            try
            {
                //_BleService.Client.StartScan(new BleDevice { Guid = "71DA3FD1-7E10-41C1-B16F-4430B5060001" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ClientViewModel.Start - Exception: {0}", ex));
            }
        }

        #endregion Actions
    }
}
