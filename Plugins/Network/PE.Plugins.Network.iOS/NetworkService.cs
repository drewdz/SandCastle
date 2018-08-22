using CoreFoundation;
using CoreTelephony;

using PE.Framework.Models;
using PE.Plugins.Network.Contracts;

using System;
using System.Threading.Tasks;
using SystemConfiguration;


namespace PE.Plugins.Network.iOS
{
    public class NetworkService : INetworkService, IDisposable
    {
        #region Events

        public event EventHandler OnConnectivityChanged;

        #endregion Events

        #region Fields

        private readonly NetworkConfiguration _Configuration;

        private NetworkReachability _Reachability;

        private string _Host = string.Empty;
        private string _Port = "80";

        #endregion Fields

        #region Constructors

        public NetworkService(NetworkConfiguration configuration)
        {
            _Configuration = configuration;
            Reset();
        }

        #endregion Constructors

        #region Properties

        public NetworkConnectionStates ConnectionState { get; set; }

        public string BaseUrl { get; set; }

        public string SecondaryUrl { get; set; }

        public bool IsOnSecondary { get; set; }

        public bool IsFailover { get; set; }

        #endregion Properties

        #region Initialization

        private async Task Init()
        {
            ConnectionState = NetworkConnectionStates.None;

            //  strip protocal and paths from host url
            var parts = BaseUrl.Split(new char[] { '/' });
            _Port = (parts[0].Equals("https")) ? "443" : "80";
            _Host = parts[2];
            //  remove port if present
            parts = _Host.Split(new char[] { ':' });
            if ((parts.Length > 1) && !string.IsNullOrEmpty(parts[1])) _Port = parts[1];
            _Host = parts[0];

            System.Diagnostics.Debug.WriteLine("Network Service - Host: {0}", _Host);

            _Reachability = new NetworkReachability(_Host);
            _Reachability.SetNotification(ReachablilityCallback);
            _Reachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);

            NetworkReachabilityFlags flags;
            if (_Reachability.TryGetFlags(out flags))
            {
                var connected = flags.HasFlag(NetworkReachabilityFlags.Reachable);
                if (connected) GetNetworkSetup(flags);
            }
        }

        #endregion Initialization

        #region Operations

        public async Task GetNetworksAsync(bool repeat, int intervalMs, Action<ServiceResult> complete)
        {
        }

        public void StopNetworkMonitor()
        {
        }

        public void Reset()
        {
            System.Diagnostics.Debug.WriteLine(string.Format("*** NetworkManager.Reset - Reset to base - {0}", _Configuration.BaseUrl));
            BaseUrl = _Configuration.BaseUrl;
            SecondaryUrl = (string.IsNullOrEmpty(_Configuration.SecondaryUrl)) ? _Configuration.BaseUrl : _Configuration.SecondaryUrl;
            Init();
        }

        #endregion Operations

        #region Private Methods

        private void ReachablilityCallback(NetworkReachabilityFlags flags)
        {
            var connected = flags.HasFlag(NetworkReachabilityFlags.Reachable);
            if (connected)
            {
                GetNetworkSetup(flags);
            }
            else
            {
                ConnectionState = NetworkConnectionStates.None;
            }
            OnConnectivityChanged?.Invoke(this, new EventArgs());
        }

        private void GetNetworkSetup(NetworkReachabilityFlags flags)
        {
            //  we're on wifi here
            if (flags.HasFlag(NetworkReachabilityFlags.IsWWAN))
            {
                //  what type of connectivity is available
                CTTelephonyNetworkInfo info = new CTTelephonyNetworkInfo();
                System.Diagnostics.Debug.WriteLine("Network Service - Cellular detected: {0}.", info.DebugDescription);
                if ((info.CurrentRadioAccessTechnology == CTRadioAccessTechnology.GPRS) || (info.CurrentRadioAccessTechnology == CTRadioAccessTechnology.Edge))
                {
                    ConnectionState = NetworkConnectionStates.CellularSlow;
                    System.Diagnostics.Debug.WriteLine("Network Service - Connection speed designated to be SLOW.");
                }
                else
                {
                    ConnectionState = NetworkConnectionStates.CellularFast;
                    System.Diagnostics.Debug.WriteLine("Network Service - Connection speed designated to be FAST.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Network Service - WIFI detected.");
                ConnectionState = NetworkConnectionStates.Wifi;
            }
        }

        #endregion Private Methods

        #region Cleanup

        public void Dispose()
        {
            if (_Reachability == null) return;
            _Reachability.Unschedule();
            _Reachability.Dispose();
        }

        #endregion Cleanup
    }
}