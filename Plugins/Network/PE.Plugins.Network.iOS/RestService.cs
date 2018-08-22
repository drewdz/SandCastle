using MvvmCross;

using PE.Framework.Models;
using PE.Framework.Serialization;
using PE.Plugins.LocalStorage;
using PE.Plugins.Network.Contracts;
using PE.Plugins.Network.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PE.Plugins.Network.iOS
{
    public class RestService : IRestService
    {
        #region Fields

        private readonly INetworkService _NetworkManager;
        private readonly ILocalStorageService _StorageService;
        private readonly NetworkConfiguration _Configuration;

        private readonly int _Timeout = 5;

        private DateTime _LastRefresh = DateTime.Now;
        private bool _IsRefreshing = false;

        #endregion Fields

        #region Constructors

        public RestService(NetworkConfiguration configuration)
        {
            _NetworkManager = Mvx.Resolve<INetworkService>();
            _StorageService = Mvx.Resolve<ILocalStorageService>();
            _Configuration = configuration;
            //  refresh the token
            if ((_Configuration.RefreshTokenCallback != null) || (_Configuration.RefreshTokenTime > 0))
            {
                RefreshToken = _Configuration.RefreshTokenCallback;
                RefreshTokenTime = _Configuration.RefreshTokenTime * 1000;
                if (RefreshTokenTime == 0) RefreshTokenTime = 590000;
            }

            //PingAsync();
        }

        #endregion Constructors

        #region Properties

        public bool Reachable { get; private set; }

        public Action RefreshToken { get; set; }

        public int RefreshTokenTime { get; set; } = 590000;

        #endregion Properties

        #region Operations

        #region Ping

        public async Task<bool> PingAsync()
        {
            try
            {
                //  ping the API
                System.Diagnostics.Debug.WriteLine(string.Format("RestService: Pinging the API...{0}", _NetworkManager.BaseUrl));
                var result = Task.Run(async () => await GetAsync<ServiceResult>("ping")).Result;
                System.Diagnostics.Debug.WriteLine(string.Format("RestService: Ping complete - {0}", result.Status));
                Reachable = (result.Status == ServiceResultStatus.Success);
                return Reachable;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("RestService: Ping failed - {0}", ex.Message));
                return false;
            }
        }

        #endregion Ping

        #region Router and startup

        public async Task<ServiceResult> RegisterForService()
        {
            try
            {
                //  make a call to the register controller to varify the service guid.
                //  this data is automatically added to the request header and returned in the same way
                return await GetAsync<ServiceResult>(_Configuration.RegisterPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("*** RestService.RegisterForService - Exception: {0}", ex);
                return new ServiceResult { Status = ServiceResultStatus.Error };
            }
        }

        #endregion Router and startup

        #region Delete

        public async Task DeleteAsync(string address)
        {
            using (HttpClient client = new HttpClient())
            {
                AddToken(client);
                var response = await client.DeleteAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)));
                CheckResponse(response);
            }
        }

        public async Task<TReturn> DeleteAsync<TReturn>(string address)
        {
            using (HttpClient client = new HttpClient())
            {
                AddToken(client);
                var response = await client.DeleteAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)));
                return await GetResponse<TReturn>(response);
            }
        }

        #endregion Delete

        #region Get

        public async Task<TReturn> GetAsync<TReturn>(string address)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);
                var response = await client.GetAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)));
                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<object> GetAsync(Type returnType, string address)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);
                var response = await client.GetAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)));
                return await GetResponse(returnType, response);
            }
        }

        public async Task<TReturn> GetAsync<TReturn>(string address, Dictionary<string, string> values)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);

                var builder = new StringBuilder(address);
                foreach (var pair in values)
                {
                    builder.Append(string.Format("{0}={1}&amp;", pair.Key, pair.Value));
                }

                var response = await client.GetAsync(new Uri(builder.ToString()));

                return await GetResponse<TReturn>(response);
            }
        }

        #endregion Get

        #region Post

        public async Task PostAsync(string address)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(string.Empty, Encoding.UTF8, Constants.CONTENT_TYPE));
                CheckResponse(response);
            }
        }

        public async Task PostAsync<TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);

                var payload = Serializer.Serialize(entity);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(payload, Encoding.UTF8, Constants.CONTENT_TYPE));

                CheckResponse(response);
            }
        }

        public async Task<TReturn> PostAsync<TReturn>(string address)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(string.Empty, Encoding.UTF8, Constants.CONTENT_TYPE));
                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<TReturn> PostAsync<TReturn, TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);

                var payload = Serializer.Serialize(entity);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(payload, Encoding.UTF8, Constants.CONTENT_TYPE));

                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<TReturn> PostFormMultipartAsync<TReturn>(string address, Dictionary<string, string> formData, string boundary = "")
        {
            //  create a boundary if necessary
            if (string.IsNullOrEmpty(boundary)) boundary = Guid.NewGuid().ToString();
            //  create content
            var content = new MultipartFormDataContent(boundary);
            if ((formData != null) && (formData.Keys.Count > 0))
            {
                foreach (var key in formData.Keys)
                {
                    content.Add(new StringContent(formData[key], Encoding.UTF8), key);
                }
            }
            //  make the call
            using (var client = new HttpClient())
            {
                AddToken(client);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _Configuration.BaseUrl, address)), content);
                return await GetResponse<TReturn>(response);
            }
        }

        #endregion Post

        #region Put

        public async Task PutAsync(string address)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);
                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(string.Empty, Encoding.UTF8, Constants.CONTENT_TYPE));
                CheckResponse(response);
            }
        }

        public async Task PutAsync<TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);
                var payload = Serializer.Serialize(entity);
                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(payload, Encoding.UTF8, Constants.CONTENT_TYPE));

                CheckResponse(response);
            }
        }

        public async Task<TReturn> PutAsync<TReturn>(string address)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);

                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(string.Empty, Encoding.UTF8, Constants.CONTENT_TYPE));

                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<TReturn> PutAsync<TReturn, TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                AddToken(client);

                var payload = Serializer.Serialize(entity);
                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new StringContent(payload, Encoding.UTF8, Constants.CONTENT_TYPE));

                return await GetResponse<TReturn>(response);
            }
        }

        #endregion Put

        #endregion Operations

        #region Initialization

        #endregion Initialization

        #region Private Methods

        private HttpResponseMessage CheckResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode && (response.StatusCode == System.Net.HttpStatusCode.Unauthorized))
            {
                throw new DeniedException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new RestException(response.ReasonPhrase);
            }
            return response;
        }

        private async Task<T> GetResponse<T>(HttpResponseMessage response)
        {
            IEnumerable<String> tokens;
            if (response.Headers.TryGetValues(_Configuration.AuthToken, out tokens))
            {
                if (!string.IsNullOrEmpty(tokens.First()))
                {
                    //  save in local storage
                    _StorageService.Put(_Configuration.AuthToken, tokens.First());
                }
            }

            if (!response.IsSuccessStatusCode && (response.StatusCode == System.Net.HttpStatusCode.Unauthorized))
            {
                throw new DeniedException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new RestException(response.ReasonPhrase);
            }

            var stream = await response.Content.ReadAsStringAsync();
            return Serializer.Deserialize<T>(stream);
        }

        private async Task<object> GetResponse(Type returnType, HttpResponseMessage response)
        {
            IEnumerable<String> tokens;
            if (response.Headers.TryGetValues(_Configuration.AuthToken, out tokens))
            {
                if (!string.IsNullOrEmpty(tokens.First()) && !string.IsNullOrEmpty(_Configuration.AuthToken))
                {
                    //  save in local storage
                    _StorageService.Put(_Configuration.AuthToken, tokens.First());
                }
            }

            if (!response.IsSuccessStatusCode && (response.StatusCode == System.Net.HttpStatusCode.Unauthorized))
            {
                throw new DeniedException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new RestException(response.ReasonPhrase);
            }

            var stream = await response.Content.ReadAsStringAsync();
            return Serializer.Deserialize(returnType, stream);
        }

        protected void AddToken(HttpClient client)
        {
            if (string.IsNullOrEmpty(_Configuration.AuthToken)) return;
            //  get the token from storage
            string token = _StorageService.Get<string>(_Configuration.AuthToken);
            if (!string.IsNullOrEmpty(token))
            {
                //  reset token validity - only once 
                //  this allows the app to die nicely if not in use for double the token validity period
                if (!_IsRefreshing) Task.Run(() => RefreshTokenAsync());
                //  get token from settings service
                if (client.DefaultRequestHeaders.Contains(_Configuration.AuthToken))
                    client.DefaultRequestHeaders.Remove(_Configuration.AuthToken);
                //  ensure that we always use the most up to date token
                client.DefaultRequestHeaders.Add(_Configuration.AuthToken, token);
            }
            client.Timeout = new TimeSpan(0, 0, _Timeout);
        }

        private async Task RefreshTokenAsync()
        {
            //  only do this if there is actually a point
            if ((RefreshToken == null) || _IsRefreshing) return;

            try
            {
                _LastRefresh = DateTime.Now;
                //  wait 10 seconds less than 10 minutes (00:09:50)
                await Task.Delay(RefreshTokenTime);
                //  some other activity has taken place in the mean-time
                if (DateTime.Now.Subtract(_LastRefresh).TotalMilliseconds < RefreshTokenTime) return;
                System.Diagnostics.Debug.WriteLine(string.Format("*** RestService.RefreshTokenAsync - Refreshing token ({0:O}", DateTime.Now));
                _IsRefreshing = true;
                //  execute code to refresh the token - usuall just a ping
                RefreshToken();
                System.Diagnostics.Debug.WriteLine(string.Format("*** RestService.RefreshTokenAsync - Done refreshing token ({0:O}", DateTime.Now));
            }
            catch { /* do nothing */ }
            finally
            {
                _IsRefreshing = false;
            }
        }

        #endregion Private Methods
    }
}
