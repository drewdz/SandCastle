using System;
using System.Linq;
using Xamarin.Auth;

namespace PE.Plugins.SecureStorage
{
	// TODO implement working with Sessions (with hashed Username and additional session details)
	public class SessionService : ISessionService
	{
		#region Fields

		private readonly SecureStorageConfiguration _Configuration;
		private readonly string _AppName;

		private AccountStore _accountStore = AccountStore.Create();

		#endregion Fields

		#region Constructors

		public SessionService(SecureStorageConfiguration configuration,
		                     string appName)
		{
			_Configuration = configuration;
			_AppName = appName;
		}

		#endregion Constructors

		#region Methods

		public void SaveCredentials(string userName, string password)
        {
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                DeleteCredentials();
                Account account = new Account
                {
                    Username = userName
                };
                account.Properties.Add("Password", password);
				_accountStore.Save(account, _AppName);
            }

        }

		public void DeleteCredentials()
        {
			var account = _accountStore.FindAccountsForService(_AppName).FirstOrDefault();
            if (account != null)
            {
				_accountStore.Delete(account, _AppName);
            }
        }

		public bool CheckCredentials(string userName, string password){
			if (DoCredentialsExist())
			{
				var account = _accountStore.FindAccountsForService(_AppName).FirstOrDefault();
                string _username = Uri.UnescapeDataString(userName);
                string _password = Uri.UnescapeDataString(password);
                return (_username.Equals(account.Username) && _password.Equals(account.Properties["Password"]));
			}
			else return false;
		}

		private bool DoCredentialsExist()
        {
			return _accountStore.FindAccountsForService(_AppName).Any() ? true : false;
        }

        #endregion Methods
	}
}
