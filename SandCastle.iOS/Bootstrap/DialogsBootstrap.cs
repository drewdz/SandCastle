using MvvmCross.Plugin;
using PE.Plugins.Dialogs.iOS;

namespace SandCastle.iOS.Bootstrap
{
    public class DialogsBootstrap : MvvmCross.Platform.Plugins.MvxPluginBootstrapAction<PE.Plugins.Dialogs.iOS.Plugin>
    {
        public static IMvxPluginConfiguration Configure()
        {
            return new iOSDialogsConfiguration();
            {
            };
        }
    }
}