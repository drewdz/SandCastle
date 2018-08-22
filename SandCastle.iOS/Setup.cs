using MvvmCross.Forms.Platforms.Ios.Core;
using MvvmCross.Plugin;

using System;

namespace SandCastle.iOS
{
    public class Setup : MvxFormsIosSetup<Core.MvxApp, Core.FormsApp>
    {
        protected override IMvxPluginConfiguration GetPluginConfiguration(Type plugin)
        {
            //  find the config method
            string name = plugin.FullName.Split(new char[] { '.' })[2];
            System.Diagnostics.Debug.WriteLine(string.Format("*** Setup.GetPluginConfiguration - Configuring plugin {0}", name));
            name = string.Format("SandCastle.iOS.Bootstrap.{0}Bootstrap", name);
            //  get this type
            var type = GetType().Assembly.GetType(name);
            if (type == null)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** Setup.GetPluginConfiguration - Setup: Could not find type {0}.", name));
                return base.GetPluginConfiguration(plugin);
            }
            //  find the configuration method
            var method = type.GetMethod("Configure", new Type[] { });
            if (method == null)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** Setup.GetPluginConfiguration - Setup: Could not find configuration method for type {0}.", name));
                return base.GetPluginConfiguration(plugin);
            }
            //  invoke the configuration method
            return (IMvxPluginConfiguration)method.Invoke(null, new object[] { });
        }
    }
}