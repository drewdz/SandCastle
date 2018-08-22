using PE.Framework.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PE.Plugins.Network.Contracts
{
    public interface IRestService
    {
        #region Properties

        bool Reachable { get; }

        Action RefreshToken { get; set; }

        int RefreshTokenTime { get; set; }

        #endregion Properties

        #region Ping

        Task<bool> PingAsync();

        #endregion Ping

        #region Router and startup

        Task<ServiceResult> RegisterForService();

        #endregion Router and startup

        #region Delete

        Task DeleteAsync(string address);

        Task<TReturn> DeleteAsync<TReturn>(string address);

        #endregion Delete

        #region Get

        Task<TReturn> GetAsync<TReturn>(string address);

        Task<object> GetAsync(Type returnType, string address);

        Task<TReturn> GetAsync<TReturn>(string address, Dictionary<string, string> values);

        #endregion Get

        #region Post

        Task PostAsync(string address);

        Task PostAsync<TEntity>(string address, TEntity entity);

        Task<TReturn> PostAsync<TReturn>(string address);

        Task<TReturn> PostAsync<TReturn, TEntity>(string address, TEntity entity);

        Task<TReturn> PostFormMultipartAsync<TReturn>(string address, Dictionary<string, string> formData, string boundary = "");

        #endregion Post

        #region Put

        Task PutAsync(string address);

        Task PutAsync<TEntity>(string address, TEntity entity);

        Task<TReturn> PutAsync<TReturn>(string address);

        Task<TReturn> PutAsync<TReturn, TEntity>(string address, TEntity entity);

        #endregion Put
    }
}
