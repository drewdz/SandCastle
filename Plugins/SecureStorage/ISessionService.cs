using System;
namespace PE.Plugins.SecureStorage
{
    public interface ISessionService
    {
		void SaveCredentials(string userName, string password);

        void DeleteCredentials();

		bool CheckCredentials(string userName, string password);
    }
}
