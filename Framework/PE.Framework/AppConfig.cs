namespace PE.Framework
{
    public class AppConfig
    {
        private static AppConfig _instance;
        public static AppConfig GetInstance()
        {
            if (_instance == null)
            {
#if RELEASE
                _instance = AppConfig.GenerateReleaseConfiguration();
#else
                _instance = AppConfig.GenerateDebugConfiguration();
#endif
            }
            return _instance;
        }

        public static AppConfig GenerateReleaseConfiguration()
        {
            AppConfig appConfig = new AppConfig()
            {
                HavingTroublesUrl = "http://pe.atelierclient.com/home/recovery",
                RegistrationUrl = "http://pe.atelierclient.com/home/sign_up",
                AppId = "Atelier",
                AppNameSuffix = "PROD",
                AuthToken = "Bearer"

            };

            return appConfig;
        }

        public static AppConfig GenerateDebugConfiguration()
        {
            AppConfig appConfig = new AppConfig()
            {
                HavingTroublesUrl = "https://enterprise.atelier.technology/home/recovery",
                RegistrationUrl = "https://enterprise.atelier.technology/home/sign_up",
                AppId = "Atelier",
                AppNameSuffix = "DEV",
                AuthToken = "Bearer"
            };

            return appConfig;
        }


        public string HavingTroublesUrl { get; set; }

        public string RegistrationUrl { get; set; }

        public string AppId { get; set; }

        public string AppNameSuffix { get; set; }

        public string AuthToken { get; set; }
    }
}
