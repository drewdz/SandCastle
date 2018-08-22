using MvvmCross.Plugin;

namespace SandCastle.UWP.Bootstrap
{
    public class DialogsBootstrap : MvvmCross.Platform.Plugins.MvxPluginBootstrapAction<PE.Plugins.Dialogs.WindowsCommon.Plugin>
    {
        public static IMvxPluginConfiguration Configure()
        {
            return new PE.Plugins.Dialogs.WindowsCommon.DialogConfig();
        }
    }
}
