using MvvmCross;

using PE.Framework.Models;
using PE.Framework.Serialization;
using PE.Plugins.LocalStorage;
using PE.Plugins.Network.Contracts;
using PE.Plugins.Network.Exceptions;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Windows.Web.Http;

namespace PE.Plugins.Network.WindowsCommon
{
    public class RestService : IRestService
    {
        #region Events

        public event EventHandler OnConnectedChanged;

        #endregion Events

        #region Fields

        private readonly INetworkService _NetworkManager;
        private readonly ILocalStorageService _LocalStorage;
        private readonly NetworkConfiguration _Configuration;

        private readonly string _RegisterPath = "register";

        #endregion Fields

        #region Constructors

        public RestService(NetworkConfiguration configuration)
        {
            _NetworkManager = Mvx.Resolve<INetworkService>();
            _LocalStorage = Mvx.Resolve<ILocalStorageService>();
            _Configuration = configuration;

            Task.Run(() => PingAsync());
        }

        #endregion Constructors

        #region Properties

        public bool Reachable { get; private set; }

        public Action RefreshToken { get; set; }

        public int RefreshTokenTime { get; set; }

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
                System.Diagnostics.Debug.WriteLine("RestService: Ping failed - {0}", ex.Message);
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
                return await GetAsync<ServiceResult>(_RegisterPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** RestService.RegisterForService - Exception: {0}", ex));
                return new ServiceResult { Status = ServiceResultStatus.Error };
            }
        }

        #endregion Router and startup

        #region Delete

        public async Task DeleteAsync(string address)
        {
            using (HttpClient client = new HttpClient())
            {
                await AddToken(client);
                var response = await client.DeleteAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)));
                CheckResponse(response);
            }
        }

        public async Task<TReturn> DeleteAsync<TReturn>(string address)
        {
            using (HttpClient client = new HttpClient())
            {
                await AddToken(client);
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
                await AddToken(client);
                var response = await client.GetAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)));
                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<object> GetAsync(Type returnType, string address)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);
                var response = await client.GetAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)));
                return await GetResponse(returnType, response);
            }
        }

        public async Task<TReturn> GetAsync<TReturn>(string address, Dictionary<string, string> values)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);

                var builder = new StringBuilder(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address));
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
                await AddToken(client);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(string.Empty, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));
                CheckResponse(response);
            }
        }

        public async Task PostAsync<TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);

                var payload = Serializer.Serialize(entity);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(payload, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));

                CheckResponse(response);
            }
        }

        public async Task<TReturn> PostAsync<TReturn>(string address)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(string.Empty, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));
                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<TReturn> PostAsync<TReturn, TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);

                var payload = Serializer.Serialize(entity);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(payload, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));

                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<TReturn> PostFormMultipartAsync<TReturn>(string address, Dictionary<string, string> formData, string boundary = "")
        {
            //  create a boundary if necessary
            if (string.IsNullOrEmpty(boundary)) boundary = Guid.NewGuid().ToString();
            //  create content
            var content = new HttpMultipartFormDataContent(boundary);
            if ((formData != null) && (formData.Keys.Count > 0))
            {
                foreach (var key  in formData.Keys)
                {
                    content.Add(new HttpStringContent(formData[key], Windows.Storage.Streams.UnicodeEncoding.Utf8), key);
                }
            }
            //  make the call
            using (var client = new HttpClient())
            {
                await AddToken(client);
                var response = await client.PostAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), content);
                return await GetResponse<TReturn>(response);
            }
        }

        #endregion Post

        #region Put

        public async Task PutAsync(string address)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);
                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(string.Empty, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));
                CheckResponse(response);
            }
        }

        public async Task PutAsync<TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);
                var payload = Serializer.Serialize(entity);
                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(payload, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));

                CheckResponse(response);
            }
        }

        public async Task<TReturn> PutAsync<TReturn>(string address)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);

                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(string.Empty, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));

                return await GetResponse<TReturn>(response);
            }
        }

        public async Task<TReturn> PutAsync<TReturn, TEntity>(string address, TEntity entity)
        {
            using (var client = new HttpClient())
            {
                await AddToken(client);

                var payload = Serializer.Serialize(entity);
                var response = await client.PutAsync(new Uri(string.Format("{0}/{1}", _NetworkManager.BaseUrl, address)), new HttpStringContent(payload, Windows.Storage.Streams.UnicodeEncoding.Utf8, Constants.CONTENT_TYPE));

                return await GetResponse<TReturn>(response);
            }
        }

        #endregion Put

        #endregion Operations

        #region Private Methods

        private HttpResponseMessage CheckResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new RestException(response.ReasonPhrase);
            }
            return response;
        }

        private async Task<TResponse> GetResponse<TResponse>(HttpResponseMessage response)
        {
            //  get the security token from the response
            string token = string.Empty;
            if (response.Headers.TryGetValue(_Configuration.AuthToken, out token))
            {
                //  TODO: save in local storage
            }
            //  get the response data
            if (!response.IsSuccessStatusCode)
            {
                throw new RestException(response.ReasonPhrase);
            }
            var content = await response.Content.ReadAsStringAsync();
            return Serializer.Deserialize<TResponse>(content);
        }

        private async Task<object> GetResponse(Type returnType, HttpResponseMessage response)
        {
            //  get the security token from the response
            string token = string.Empty;
            if (response.Headers.TryGetValue(_Configuration.AuthToken, out token))
            {
                //  save in local storage
                _LocalStorage.Put(_Configuration.AuthToken, token);
            }
            //  get the response data
            if (!response.IsSuccessStatusCode)
            {
                throw new RestException(response.ReasonPhrase);
            }
            var content = await response.Content.ReadAsStringAsync();
            return Serializer.Deserialize(returnType, content);
        }

        private async Task AddToken(HttpClient client)
        {
            //  get the token from storage
            string token = _LocalStorage.Get<string>(_Configuration.AuthToken);
            if (!string.IsNullOrEmpty(token))
            {
                //  get token from settings service
                if (client.DefaultRequestHeaders.ContainsKey(_Configuration.AuthToken))
                    client.DefaultRequestHeaders.Remove(_Configuration.AuthToken);
                //  ensure that we always use the most up to date token
                client.DefaultRequestHeaders.Add(_Configuration.AuthToken, token);
            }
        }

        #endregion Private Methods
    }
}
