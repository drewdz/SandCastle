using MvvmCross.Plugin;
using System;

namespace PE.Plugins.Network
{
    public class NetworkConfiguration : IMvxPluginConfiguration
    {
        /// <summary>
        /// Base URL for all requests
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Secondary url to use if the base url fails. If applicable
        /// </summary>
        public string SecondaryUrl { get; set; }

        /// <summary>
        /// Path to the register controller to register the service id
        /// </summary>
        public string RegisterPath { get; set; }

        /// <summary>
        /// How long in seconds before a request times out
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Name of the Authorization token in the request header
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// An action to call to refresh the token if it about to expire
        /// </summary>
        /// <value>The refresh token callbacl.</value>
        public Action RefreshTokenCallback { get; set; }

        /// <summary>
        /// Number of seconds before the token is refreshed when no other activity has taken place. The token will only be refreshed once; if the app is still inactivie it will be allowed to die.
        /// </summary>
        /// <value>The token expiry timout.</value>
        public int RefreshTokenTime { get; set; }
    }
}
