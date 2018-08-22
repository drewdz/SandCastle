using MvvmCross.Commands;
using MvvmCross.ViewModels;
//using PE.Plugins.Bluetooth;
using System;
using System.Threading.Tasks;

namespace SandCastle.Core.ViewModels
{
    //  SERVER
    //  Generic Access	org.bluetooth.service.generic_access	0x1800
    //  Automation IO	org.bluetooth.service.automation_io	0x1815
    //  Human Interface Device	org.bluetooth.service.human_interface_device	0x1812
    //  Scan Parameters	org.bluetooth.service.scan_parameters	0x1813

    //  CHARACTERISTICS
    //  Object Changed	org.bluetooth.characteristic.object_changed	0x2AC8
    public class ServerViewModel : MvxViewModel
    {
        #region Fields

        //private readonly IBleService _BleService;

        private DateTime _Timestamp;

        #endregion Fields

        #region Constructors

        public ServerViewModel()
        {
            //_BleService = bleService;
        }

        #endregion Constructors

        #region Commands

        private IMvxCommand _StartCommand = null;
        public IMvxCommand StartCommand => _StartCommand ?? new MvxCommand(() =>
        {
            //_BleService.Server.Start(new BleDevice { GuidValue = 0x1813, Name = "Test" }, new BleCharacteristic[] {
            //    new BleCharacteristic { GuidValue = 0x2AC8, Name = "Changed" }
            //});
            CanStart = false;
            Prompt = "Server has been started. We're broadcasting!";
        });

        #endregion Commands

        #region Properties

        private string _Text = string.Empty;
        public string Text
        {
            get => _Text;
            set
            {
                SetProperty(ref _Text, value);
                Task.Run(() => SendText());
            }
        }

        private string _Prompt = "Ready to start the Bluetooth Low Energy Server?";
        public string Prompt
        {
            get => _Prompt;
            set => SetProperty(ref _Prompt, value);
        }

        private bool _CanStart = true;
        public bool CanStart
        {
            get => _CanStart;
            set => SetProperty(ref _CanStart, value);
        }

        #endregion Properties

        #region Operations

        private async Task SendText()
        {
            try
            {
                _Timestamp = DateTime.Now;
                await Task.Delay(700);
                if (DateTime.Now.Subtract(_Timestamp).TotalMilliseconds < 700) return;
                //  TODO: send the text
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("*** ServerViewModel.SendText - Exception: {0}", ex);
            }
        }

        #endregion Operations
    }
}
