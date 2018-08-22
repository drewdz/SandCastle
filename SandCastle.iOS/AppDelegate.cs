using Foundation;
using MvvmCross.Forms.Platforms.Ios.Core;
using UIKit;

namespace SandCastle.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxFormsApplicationDelegate<Setup, Core.MvxApp, UI.App>
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.MakeKeyAndVisible();

            return base.FinishedLaunching(app, options);
        }

        public override void OnResignActivation(UIApplication application)
        {
            base.OnResignActivation(application);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            base.DidEnterBackground(application);
        }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);
        }

        public override void OnActivated(UIApplication application)
        {
            base.OnActivated(application);
        }

        public override void WillTerminate(UIApplication application)
        {
            base.WillTerminate(application);
        }
    }
}


