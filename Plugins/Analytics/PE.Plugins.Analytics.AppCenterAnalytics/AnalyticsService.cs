using System;
using System.Collections.Generic;
using PE.Framework.AppVersion;
using static PE.Plugins.Analytics.Constants;

namespace PE.Plugins.Analytics.AppCenterAnalytics
{
    public abstract class AnalyticsService : IAnalyticsService
    {
        #region Constants

        private const string VIEW_NAME = "ViewName";
        private const string ADDITIONAL_VALUE = "AdditionalValue";
        private const string TRACKING_NUMBER = "TrackingNumber";
        private const string EX_STACK_TRACK = "StackTrace";
        private const string EX_MESSAGE = "Message";
        private const string EX_EXCEPTION = "Exception";
        private const string SESSION_START = "SessionStart";
        private const string USER_ID = "UserID";

        #endregion Constants

        #region Fields

        protected readonly AppCenterConfiguration _Configuration;
		protected IVersion _version;
        #endregion Fields

        #region Constructors

        public AnalyticsService(AppCenterConfiguration configuration)
        {
            _Configuration = configuration;         
        }

        #endregion Constructors

        #region Operations

        public void RecordEvent(string eventType, string viewName, string additionalValue)
        {
            var values = (string.IsNullOrEmpty(additionalValue)) ? null : new Dictionary<string, string> { { ADDITIONAL_VALUE, additionalValue } };
            RecordEvent(eventType, viewName, values);
        }

        public void RecordEvent(string eventType, string viewName, Dictionary<string, string> values)
        {
            try
            {
                if ((values == null) || (values.Count == 0))
                {
                    values = new Dictionary<string, string> { { VIEW_NAME, viewName } };
                }
                else
                {
                    if (!values.ContainsKey(VIEW_NAME)) values.Add(VIEW_NAME, viewName);
                }
				values.Add(APPVERSION, _version.Version);
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventType, values);
            }
            catch (Exception ex)
            {
                //  do nothing
                System.Diagnostics.Debug.WriteLine(string.Format("*** Analytics.RecordException - Exception: {0}", ex));
            }
        }

        public void RecordException(Exception exception, string viewName, string trackingNumber)
        {
            //  create additional fields
            var values = new Dictionary<string, string>()
            {
                { TRACKING_NUMBER, (string.IsNullOrEmpty(trackingNumber)) ? "[Unknown]" : trackingNumber },
                { EX_MESSAGE, exception.Message },
                { EX_STACK_TRACK, exception.StackTrace }
            };
            RecordEvent(EX_EXCEPTION, viewName, values);
        }

        public void SessionStart(string userId)
        {
            RecordEvent(SESSION_START, string.Empty, new Dictionary<string, string> { { USER_ID, userId } });
        }

        //public void StartAnalytics()
        //{
        //    try
        //    {
        //        _Configuration.StartCallback?.Invoke();
        //    }
        //    catch (Exception ex)
        //    {
        //        //  do nothing
        //        System.Diagnostics.Debug.WriteLine(string.Format("*** Analytics.StartAnalytics - Exception: {0}", ex));
        //    }
        //}

        public abstract void StartAnalytics();

        #endregion  Operations
    }
}
