using System;
namespace PE.Plugins.Analytics
{
    public static class Constants
    {
		public const string APPVERSION = "APPVERSION";

		#region Login

		public const string LOGIN = "LOGIN";
		public const string LOGIN_ATTEMPT = "LOGIN_ATTEMPT";
		public const string LOGIN_SUCCESSFUL = "LOGIN_SUCCESSFUL";
		public const string LOGIN_SUCCESS_NEED_2FA = "LOGIN_SUCCESS_NEED_2FA";
		public const string LOGIN_REJECTED = "LOGIN_REJECTED";
		public const string LOGIN_FAILED = "LOGIN_FAILED";
		public const string LOGIN_NO_INTERNET = "LOGIN_NO_INTERNET";      
		public const string TWOFACTOR_SUBMIT_PHONE_NUMBER_SUCCESS = "TWOFACTOR_SUBMIT_PHONE_NUMBER_SUCCESS";
		public const string TWOFACTOR_SUBMIT_PHONE_NUMBER_REJECTED = "TWOFACTOR_SUBMIT_PHONE_NUMBER_REJECTED";
		public const string TWOFACTOR_SUBMIT_SMS_CODE_SUCCESS = "TWOFACTOR_SUBMIT_SMS_CODE_SUCCESS";
		public const string TWOFACTOR_SUBMIT_SMS_CODE_REJECTED = "TWOFACTOR_SUBMIT_SMS_CODE_REJECTED";
		public const string TWOFACTOR_RESEND_SMS_CODE = "TWOFACTOR_RESEND_SMS_CODE";

        #endregion Login
    }
}
